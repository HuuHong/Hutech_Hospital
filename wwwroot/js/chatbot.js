/**
 * HUTECH_Hospital AI Chatbot Widget
 * Powered by Google Gemini
 */
(function () {
    'use strict';

    // ── Quick reply suggestions ──────────────────────────────
    const QUICK_REPLIES = [
        "Đặt lịch khám như thế nào?",
        "Các chuyên khoa có ở bệnh viện?",
        "Bảng giá dịch vụ?",
        "Giờ làm việc bệnh viện?",
    ];

    // ── Bot welcome message ──────────────────────────────────
    const WELCOME_MSG = `Xin chào! 👋 Tôi là **Trợ lý AI HUTECH_Hospital**.

Tôi có thể hỗ trợ bạn về:
• Đặt lịch khám bệnh
• Thông tin các chuyên khoa
• Tư vấn sức khỏe ban đầu
• Bảng giá dịch vụ

Bạn cần hỗ trợ gì hôm nay?`;

    // ── SVG Icons ─────────────────────────────────────────────
    const ICONS = {
        // Medical cross + AI circuit icon
        fab: `<svg viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
  <!-- Stethoscope shape -->
  <circle cx="12" cy="12" r="10.5" stroke="rgba(255,255,255,0.25)" stroke-width="1" fill="none"/>
  <!-- Cross -->
  <rect x="10.5" y="5.5" width="3" height="13" rx="1.5" fill="white"/>
  <rect x="5.5" y="10.5" width="13" height="3" rx="1.5" fill="white"/>
  <!-- Center sparkle -->
  <circle cx="12" cy="12" r="2" fill="rgba(255,255,255,0.4)"/>
  <!-- AI dots -->
  <circle cx="6" cy="6" r="1" fill="rgba(255,255,255,0.6)"/>
  <circle cx="18" cy="6" r="1" fill="rgba(255,255,255,0.6)"/>
  <circle cx="6" cy="18" r="1" fill="rgba(255,255,255,0.6)"/>
  <circle cx="18" cy="18" r="1" fill="rgba(255,255,255,0.6)"/>
</svg>`,
        bot: `<svg viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg" width="16" height="16">
  <rect x="5.5" y="9.5" width="13" height="3.5" rx="1.5" fill="white" opacity="0.9"/>
  <rect x="10.5" y="6" width="3" height="12" rx="1.5" fill="white" opacity="0.9"/>
</svg>`,
        send: `<svg viewBox="0 0 24 24" fill="currentColor"><path d="M2.01 21L23 12 2.01 3 2 10l15 2-15 2z"/></svg>`,
        close: `<svg viewBox="0 0 24 24" fill="currentColor" width="14" height="14"><path d="M19 6.41L17.59 5 12 10.59 6.41 5 5 6.41 10.59 12 5 17.59 6.41 19 12 13.41 17.59 19 19 17.59 13.41 12z"/></svg>`,
        user: `<svg viewBox="0 0 24 24" fill="currentColor" width="14" height="14"><path d="M12 12c2.7 0 4.8-2.1 4.8-4.8S14.7 2.4 12 2.4 7.2 4.5 7.2 7.2 9.3 12 12 12zm0 2.4c-3.2 0-9.6 1.6-9.6 4.8v2.4h19.2v-2.4c0-3.2-6.4-4.8-9.6-4.8z"/></svg>`,
        gemini: `<svg viewBox="0 0 28 28" fill="none" width="11" height="11"><path d="M14 2C8.477 2 4 6.477 4 12s4.477 10 10 10 10-4.477 10-10S19.523 2 14 2zm0 18c-4.418 0-8-3.582-8-8s3.582-8 8-8 8 3.582 8 8-3.582 8-8 8z" fill="rgba(255,255,255,0.7)"/><circle cx="14" cy="12" r="3" fill="white"/></svg>`,
    };

    // ── Build DOM ─────────────────────────────────────────────
    function buildChatbot() {
        // FAB Button
        const fab = document.createElement('button');
        fab.id = 'chatbot-fab';
        fab.setAttribute('aria-label', 'Mở chat với AI trợ lý');
        fab.innerHTML = ICONS.fab + `<span id="chatbot-badge">1</span>`;
        document.body.appendChild(fab);

        // Chat Window
        const win = document.createElement('div');
        win.id = 'chatbot-window';
        win.setAttribute('role', 'dialog');
        win.setAttribute('aria-label', 'Trợ lý AI HUTECH_Hospital');
        win.innerHTML = `
            <div id="chatbot-header">
                <div class="chatbot-header-avatar">${ICONS.bot}</div>
                <div class="chatbot-header-info">
                    <div class="chatbot-header-name">Trợ lý AI HUTECH</div>
                    <div class="chatbot-header-status">
                        <span class="status-dot"></span> Sẵn sàng hỗ trợ
                    </div>
                </div>
                <button id="chatbot-close-btn" aria-label="Đóng chat">${ICONS.close}</button>
            </div>

            <div id="chatbot-messages"></div>

            <div id="chatbot-quick-replies"></div>

            <div id="chatbot-input-area">
                <textarea id="chatbot-input" rows="1" placeholder="Nhập câu hỏi..."></textarea>
                <button id="chatbot-send-btn" aria-label="Gửi">${ICONS.send}</button>
            </div>`;
        document.body.appendChild(win);

        // Wire events
        fab.addEventListener('click', toggleChat);
        document.getElementById('chatbot-close-btn').addEventListener('click', closeChat);
        document.getElementById('chatbot-send-btn').addEventListener('click', sendMessage);
        document.getElementById('chatbot-input').addEventListener('keydown', function (e) {
            if (e.key === 'Enter' && !e.shiftKey) { e.preventDefault(); sendMessage(); }
        });
        document.getElementById('chatbot-input').addEventListener('input', autoResize);

        // Quick replies
        const qr = document.getElementById('chatbot-quick-replies');
        QUICK_REPLIES.forEach(function (label) {
            const btn = document.createElement('button');
            btn.className = 'quick-reply-btn';
            btn.textContent = label;
            btn.addEventListener('click', function () { sendQuickReply(label); });
            qr.appendChild(btn);
        });

        // Welcome message
        appendBotMessage(WELCOME_MSG);
    }

    // ── Helpers ───────────────────────────────────────────────
    let isOpen = false;
    let jwtToken = null;

    async function getJwtToken() {
        if (jwtToken) return jwtToken;
        try {
            const res = await fetch('/api/auth/session-token');
            if (res.ok) {
                const data = await res.json();
                jwtToken = data.token;
                return jwtToken;
            } else if (res.status === 401) {
                return null; // Not logged in
            }
        } catch (e) {
            console.error("Error fetching token:", e);
        }
        return null;
    }

    function toggleChat() {
        isOpen ? closeChat() : openChat();
    }

    function openChat() {
        isOpen = true;
        document.getElementById('chatbot-window').classList.add('open');
        const badge = document.getElementById('chatbot-badge');
        if (badge) badge.remove();
        document.getElementById('chatbot-input').focus();
        scrollToBottom();
    }

    function closeChat() {
        isOpen = false;
        document.getElementById('chatbot-window').classList.remove('open');
    }

    function autoResize() {
        const el = document.getElementById('chatbot-input');
        el.style.height = 'auto';
        el.style.height = Math.min(el.scrollHeight, 90) + 'px';
    }

    function scrollToBottom() {
        const msgs = document.getElementById('chatbot-messages');
        if (msgs) msgs.scrollTop = msgs.scrollHeight;
    }

    // ── Message rendering ─────────────────────────────────────
    function formatMarkdown(text) {
        return text
            .replace(/\*\*(.*?)\*\*/g, '<strong>$1</strong>')
            .replace(/\*(.*?)\*/g, '<em>$1</em>')
            .replace(/^• /gm, '&bull; ')
            .replace(/\n/g, '<br>');
    }

    function appendBotMessage(text) {
        const msgs = document.getElementById('chatbot-messages');
        const div = document.createElement('div');
        div.className = 'cb-msg bot';
        div.innerHTML = `
            <div class="cb-avatar">${ICONS.bot}</div>
            <div class="cb-bubble">${formatMarkdown(text)}</div>`;
        msgs.appendChild(div);
        scrollToBottom();
    }

    function appendUserMessage(text) {
        const msgs = document.getElementById('chatbot-messages');
        const div = document.createElement('div');
        div.className = 'cb-msg user';
        div.innerHTML = `
            <div class="cb-avatar">${ICONS.user}</div>
            <div class="cb-bubble">${escapeHtml(text)}</div>`;
        msgs.appendChild(div);
        scrollToBottom();
    }

    function showTyping() {
        const msgs = document.getElementById('chatbot-messages');
        const div = document.createElement('div');
        div.className = 'cb-msg bot';
        div.id = 'cb-typing-indicator';
        div.innerHTML = `
            <div class="cb-avatar">${ICONS.bot}</div>
            <div class="cb-bubble cb-typing">
                <span></span><span></span><span></span>
            </div>`;
        msgs.appendChild(div);
        scrollToBottom();
    }

    function hideTyping() {
        const el = document.getElementById('cb-typing-indicator');
        if (el) el.remove();
    }

    function escapeHtml(str) {
        return str.replace(/&/g,'&amp;').replace(/</g,'&lt;').replace(/>/g,'&gt;');
    }

    // ── Send message ──────────────────────────────────────────
    function sendQuickReply(text) {
        document.getElementById('chatbot-input').value = text;
        sendMessage();
    }

    async function sendMessage() {
        const input = document.getElementById('chatbot-input');
        const text = input.value.trim();
        if (!text) return;

        // Clear quick replies on first user message
        const qr = document.getElementById('chatbot-quick-replies');
        if (qr) { qr.innerHTML = ''; }

        appendUserMessage(text);
        input.value = '';
        input.style.height = 'auto';

        const sendBtn = document.getElementById('chatbot-send-btn');
        sendBtn.disabled = true;
        showTyping();

        const token = await getJwtToken();
        if (!token) {
            hideTyping();
            appendBotMessage('⚠️ Hệ thống bảo mật: Bạn cần **[Đăng nhập](/Account/Login)** để tiếp tục sử dụng Trợ lý AI.');
            sendBtn.disabled = false;
            scrollToBottom();
            return;
        }

        fetch('/api/Chatbot/ask', {
            method: 'POST',
            headers: { 
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${token}` 
            },
            body: JSON.stringify({ message: text })
        })
        .then(function (r) { 
            if (r.status === 401) {
                jwtToken = null; // Token expired maybe
                return { reply: 'Phiên bản làm việc hết hạn. Vui lòng **[Đăng nhập lại](/Account/Login)**.' };
            }
            return r.json(); 
        })
        .then(function (data) {
            hideTyping();
            appendBotMessage(data.reply || 'Xin lỗi, tôi không thể trả lời ngay lúc này.');
        })
        .catch(function () {
            hideTyping();
            appendBotMessage('❌ Mất kết nối API. Vui lòng thử lại sau.');
        })
        .finally(function () {
            sendBtn.disabled = false;
            scrollToBottom();
        });
    }

    // ── Init ──────────────────────────────────────────────────
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', buildChatbot);
    } else {
        buildChatbot();
    }
})();
