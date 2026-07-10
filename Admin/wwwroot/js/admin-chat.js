/* Messenger dock (Beehive buddy-chat markup, jQuery-driven).
   Lets signed-in admins message each other: buddy rail on the extreme right,
   pop-up chat windows, Enter-to-send, unread badges, and SignalR push updates. */
(function ($) {
    'use strict';

    var $app = $('#buddy-chat-app');
    if (!$app.length) { return; }

    var urls = {
        buddies: $app.data('buddies-url'),
        history: $app.data('history-url'),
        send: $app.data('send-url'),
        updates: $app.data('updates-url'),
        markRead: $app.data('markread-url')
    };
    var currentUserId = String($app.data('current-user') || '');
    var token = $app.find('input[name="__RequestVerificationToken"]').val();

    var buddies = [];
    var openWindows = {};   // userId -> { $el, lastMessageId }
    var maxWindows = 3;
    var soundMuted = window.localStorage.getItem('bpc-muted') === '1';
    var hubConnection = null;

    /* ---------- helpers ---------- */

    function esc(text) {
        return $('<span>').text(text == null ? '' : String(text)).html();
    }

    function timeLabel(iso) {
        var d = new Date(iso);
        if (isNaN(d)) { return ''; }
        var today = new Date();
        var sameDay = d.toDateString() === today.toDateString();
        var hm = d.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });
        return sameDay ? hm : d.toLocaleDateString() + ' ' + hm;
    }

    function buddyById(id) {
        for (var i = 0; i < buddies.length; i++) {
            if (String(buddies[i].id) === String(id)) { return buddies[i]; }
        }
        return null;
    }

    function postJson(url, payload) {
        return $.ajax({
            url: url,
            method: 'POST',
            contentType: 'application/json',
            headers: { 'RequestVerificationToken': token },
            data: JSON.stringify(payload)
        });
    }

    function initSignalR() {
        if (hubConnection) { return; }

        hubConnection = new signalR.HubConnectionBuilder()
            .withUrl('/chathub')
            .withAutomaticReconnect()
            .build();

        hubConnection.on('ReceiveMessage', function (message) {
            var senderId = String(message.senderId);
            var win = openWindows[senderId];
            if (win) {
                appendMessage(win.$el, message, buddyById(senderId));
                win.lastMessageId = Math.max(win.lastMessageId, message.id || 0);
                scrollToBottom(win.$el);
                postJson(urls.markRead, { userId: senderId });
            }

            var buddy = buddyById(senderId);
            if (!buddy) { return; }

            buddy.unread = (buddy.unread || 0) + 1;
            renderBuddies($('#bpc-buddy-filter').val());
            updateRailBadge();
            notifySound();
        });

        hubConnection.start().catch(function () {
            // Fall back to the existing REST polling if SignalR cannot start.
            poll();
        });
    }

    function notifySound() {
        if (soundMuted) { return; }
        try {
            var ctx = window.__bpcAudioCtx || (window.__bpcAudioCtx = new (window.AudioContext || window.webkitAudioContext)());
            var osc = ctx.createOscillator();
            var gain = ctx.createGain();
            osc.frequency.value = 880;
            gain.gain.setValueAtTime(0.06, ctx.currentTime);
            gain.gain.exponentialRampToValueAtTime(0.0001, ctx.currentTime + 0.25);
            osc.connect(gain).connect(ctx.destination);
            osc.start();
            osc.stop(ctx.currentTime + 0.25);
        } catch (e) { /* audio optional */ }
    }

    /* ---------- buddies rail ---------- */

    function renderBuddies(filterText) {
        var $list = $('#bpc-buddy-items').empty();
        var needle = (filterText || '').toLowerCase();
        var shown = 0;

        $.each(buddies, function (_, b) {
            if (needle && b.name.toLowerCase().indexOf(needle) === -1) { return; }
            shown++;
            var $item = $(
                '<div class="bpc-item" role="button" tabindex="0">' +
                '  <div class="avatar-container">' +
                '    <img class="avatar" alt="" />' +
                '    <span class="status"></span>' +
                '  </div>' +
                '  <div class="bpc-item-body">' +
                '    <div class="flex-r">' +
                '      <div class="buddy"><div class="chat-buddy anchor ellipsis"></div></div>' +
                '    </div>' +
                '  </div>' +
                '</div>');
            $item.attr('data-user-id', b.id);
            $item.find('img.avatar').attr('src', b.avatar).attr('alt', b.name);
            $item.find('.chat-buddy').text(b.name);
            if (b.online) { $item.find('.status').addClass('online'); }
            if (b.unread > 0) {
                $item.find('.bpc-item-body .flex-r').append(
                    $('<span class="bpc-unread-badge"></span>').text(b.unread > 99 ? '99+' : b.unread));
            }
            $list.append($item);
        });

        $('.bpc-empty').toggle(shown === 0);
    }

    function loadBuddies() {
        $.getJSON(urls.buddies)
            .done(function (data) {
                buddies = data || [];
                $('.bpc-loading').hide();
                renderBuddies($('#bpc-buddy-filter').val());
                updateRailBadge();
            })
            .fail(function () {
                $('.bpc-loading .bpc-notice').text('Could not load administrators.');
            });
    }

    function updateRailBadge() {
        var total = 0;
        $.each(buddies, function (_, b) { total += (b.unread || 0); });
        var $title = $('#buddy-chat-buddies .header-title');
        $title.find('.bpc-rail-total').remove();
        if (total > 0) {
            $title.append($('<span class="bpc-rail-total"></span>').text(total > 99 ? '99+' : total));
        }
    }

    /* ---------- chat windows ---------- */

    function windowTemplate(buddy) {
        return $(
            '<li class="chat-window">' +
            '  <div class="chat-window__container">' +
            '    <div class="chat-window__title">' +
            '      <div class="avatar-container">' +
            '        <img class="avatar" src="' + esc(buddy.avatar) + '" alt="' + esc(buddy.name) + '" />' +
            '        <span class="status' + (buddy.online ? ' online' : '') + '"></span>' +
            '      </div>' +
            '      <span class="window-buddy-name">' + esc(buddy.name) + '</span>' +
            '      <a class="chat_window__close-btn" role="button" aria-label="Close chat">' +
            '        <span class="dashicons dashicons-no-alt"></span>' +
            '      </a>' +
            '    </div>' +
            '    <div class="chat-window__message-list">' +
            '      <ul class="bpc-chat-list"></ul>' +
            '    </div>' +
            '    <div class="chat-window__inputarea">' +
            '      <div class="chat-window__input">' +
            '        <div class="chat-window__input--field" contenteditable="true" role="textbox" aria-label="Write a message" data-placeholder="Write a message..."></div>' +
            '      </div>' +
            '      <div class="chat-window__btn--enter" role="button" aria-label="Send">' +
            '        <span><span class="dashicons dashicons-migrate"></span></span>' +
            '      </div>' +
            '    </div>' +
            '  </div>' +
            '</li>');
    }

    function appendMessage($win, msg, buddy) {
        var self = String(msg.senderId) === currentUserId;
        var $list = $win.find('.bpc-chat-list');
        var $li = $('<li class="' + (self ? 'message--self' : 'message--other') + '"></li>');
        var $block = $('<div class="message-block"></div>');
        if (!self && buddy) {
            $block.append('<img class="avatar" src="' + esc(buddy.avatar) + '" alt="' + esc(buddy.name) + '" />');
        }
        var $messages = $('<div class="messages"></div>');
        $messages.append('<div class="message"><span>' + esc(msg.content) + '</span></div>');
        $block.append($messages);
        $li.append($block);
        $li.append('<time datetime="' + esc(msg.sentAt) + '">' + esc(timeLabel(msg.sentAt)) + '</time>');
        $list.append($li);
    }

    function scrollToBottom($win) {
        var el = $win.find('.chat-window__message-list')[0];
        if (el) { el.scrollTop = el.scrollHeight; }
    }

    function openChat(userId) {
        var buddy = buddyById(userId);
        if (!buddy) { return; }

        if (openWindows[userId]) {
            openWindows[userId].$el.addClass('jump');
            setTimeout(function () { openWindows[userId] && openWindows[userId].$el.removeClass('jump'); }, 600);
            openWindows[userId].$el.find('.chat-window__input--field').trigger('focus');
            return;
        }

        var ids = Object.keys(openWindows);
        if (ids.length >= maxWindows) {
            closeChat(ids[0]);
        }

        var $win = windowTemplate(buddy);
        $win.attr('data-user-id', userId);
        $('#buddy-chat-windows .bpc-chat-windows-list').append($win);
        openWindows[userId] = { $el: $win, lastMessageId: 0 };

        $.getJSON(urls.history, { userId: userId })
            .done(function (messages) {
                $.each(messages || [], function (_, m) {
                    appendMessage($win, m, buddy);
                    if (m.id > openWindows[userId].lastMessageId) {
                        openWindows[userId].lastMessageId = m.id;
                    }
                });
                scrollToBottom($win);
                buddy.unread = 0;
                renderBuddies($('#bpc-buddy-filter').val());
                updateRailBadge();
            });

        $win.find('.chat-window__input--field').trigger('focus');
    }

    function closeChat(userId) {
        if (openWindows[userId]) {
            openWindows[userId].$el.remove();
            delete openWindows[userId];
        }
    }

    function sendFrom($win) {
        var userId = String($win.attr('data-user-id'));
        var $field = $win.find('.chat-window__input--field');
        var text = $.trim($field.text());
        if (!text) { return; }

        $field.text('');
        if (hubConnection && hubConnection.state === signalR.HubConnectionState.Connected) {
            hubConnection.invoke('SendMessage', userId, text)
                .then(function (msg) {
                    appendMessage($win, msg, buddyById(userId));
                    if (openWindows[userId] && msg.id > openWindows[userId].lastMessageId) {
                        openWindows[userId].lastMessageId = msg.id;
                    }
                    scrollToBottom($win);
                })
                .catch(function () {
                    postJson(urls.send, { recipientId: userId, content: text })
                        .done(function (msg) {
                            appendMessage($win, msg, buddyById(userId));
                            if (openWindows[userId] && msg.id > openWindows[userId].lastMessageId) {
                                openWindows[userId].lastMessageId = msg.id;
                            }
                            scrollToBottom($win);
                        })
                        .fail(function () {
                            var $list = $win.find('.bpc-chat-list');
                            $list.append('<li class="message--self"><div class="message-block"><div class="messages"><div class="message bpc-send-failed"><span>Message failed to send.</span></div></div></div></li>');
                            scrollToBottom($win);
                        });
                });
            return;
        }

        postJson(urls.send, { recipientId: userId, content: text })
            .done(function (msg) {
                appendMessage($win, msg, buddyById(userId));
                if (openWindows[userId] && msg.id > openWindows[userId].lastMessageId) {
                    openWindows[userId].lastMessageId = msg.id;
                }
                scrollToBottom($win);
            })
            .fail(function () {
                var $list = $win.find('.bpc-chat-list');
                $list.append('<li class="message--self"><div class="message-block"><div class="messages"><div class="message bpc-send-failed"><span>Message failed to send.</span></div></div></div></li>');
                scrollToBottom($win);
            });
    }

    /* ---------- fallback polling ---------- */

    function poll() {
        if (hubConnection && hubConnection.state === signalR.HubConnectionState.Connected) { return; }

        $.getJSON(urls.updates, { sinceId: 0 })
            .done(function (data) {
                if (!data) { return; }

                var unreadMap = {};
                $.each(data.unread || [], function (_, u) { unreadMap[String(u.senderId)] = u.count; });

                $.each(data.messages || [], function (_, m) {
                    var senderId = String(m.senderId);
                    var win = openWindows[senderId];
                    if (win) {
                        appendMessage(win.$el, m, buddyById(senderId));
                        win.lastMessageId = Math.max(win.lastMessageId, m.id || 0);
                        scrollToBottom(win.$el);
                        postJson(urls.markRead, { userId: senderId });
                    }
                });

                $.each(buddies, function (_, b) {
                    b.unread = unreadMap[String(b.id)] || 0;
                });
                renderBuddies($('#bpc-buddy-filter').val());
                updateRailBadge();
            });
    }

    /* ---------- events ---------- */

    // Collapse / expand the buddies rail
    $app.on('click', '#buddy-chat-buddies__collapser', function () {
        $('#buddy-chat-buddies').toggleClass('collapsed');
    });
    $app.on('click', '#buddy-chat-buddies.collapsed .header-container', function () {
        $('#buddy-chat-buddies').removeClass('collapsed');
    });

    // Settings dropdown (mute)
    $app.on('click', '.dropd-control', function (e) {
        e.preventDefault();
        e.stopPropagation();
        $(this).closest('.dropd-group').toggleClass('active');
    });
    $(document).on('click', function () {
        $app.find('.dropd-group').removeClass('active');
    });
    $app.on('click', '#bpc-toggle-sound', function (e) {
        e.preventDefault();
        soundMuted = !soundMuted;
        window.localStorage.setItem('bpc-muted', soundMuted ? '1' : '0');
        $(this).text(soundMuted ? 'Unmute' : 'Mute');
        $(this).closest('.dropd-group').removeClass('active');
    });

    // Buddy click -> open chat
    $app.on('click keydown', '.bpc-item[data-user-id]', function (e) {
        if (e.type === 'keydown' && e.key !== 'Enter' && e.key !== ' ') { return; }
        e.preventDefault();
        openChat(String($(this).attr('data-user-id')));
    });

    // Buddy filter
    $('#bpc-buddy-filter').on('input', function () {
        renderBuddies($(this).val());
    });

    // Window actions
    $app.on('click', '.chat_window__close-btn', function () {
        closeChat(String($(this).closest('.chat-window').attr('data-user-id')));
    });
    $app.on('click', '.chat-window__btn--enter', function () {
        sendFrom($(this).closest('.chat-window'));
    });
    $app.on('keydown', '.chat-window__input--field', function (e) {
        if (e.key === 'Enter' && !e.shiftKey) {
            e.preventDefault();
            sendFrom($(this).closest('.chat-window'));
        }
    });
    // Focusing a window clears that buddy's unread state
    $app.on('focusin', '.chat-window', function () {
        var userId = String($(this).attr('data-user-id'));
        var buddy = buddyById(userId);
        if (buddy && buddy.unread > 0) {
            buddy.unread = 0;
            renderBuddies($('#bpc-buddy-filter').val());
            updateRailBadge();
            if (hubConnection && hubConnection.state === signalR.HubConnectionState.Connected) {
                hubConnection.invoke('MarkAsRead', userId).catch(function () { });
            } else {
                postJson(urls.markRead, { userId: userId });
            }
        }
    });

    /* ---------- boot ---------- */

    $(function () {
        loadBuddies();
        initSignalR();
        poll();
    });
})(jQuery);
