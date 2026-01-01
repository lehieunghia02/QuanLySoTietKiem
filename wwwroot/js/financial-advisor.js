// Voice Input
let isRecording = false;
const recognition = new webkitSpeechRecognition();
recognition.continuous = false;
recognition.lang = 'vi-VN';

function toggleVoiceInput() {
    if (!isRecording) {
        recognition.start();
    } else {
        recognition.stop();
    }
    isRecording = !isRecording;
    document.querySelector('.voice-input-btn').classList.toggle('active');
}

// Export Chat
function exportChat() {
    const messages = Array.from(document.querySelectorAll('.message')).map(msg => {
        return {
            content: msg.textContent,
            type: msg.classList.contains('user-message') ? 'user' : 'bot',
            timestamp: new Date().toISOString()
        };
    });

    const blob = new Blob([JSON.stringify(messages, null, 2)], { type: 'application/json' });
    const url = URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = 'chat-history.json';
    a.click();
}

// Rate Response
async function rateResponse(rating) {
    try {
        await fetch('/FinancialAdvisor/RateResponse', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ rating })
        });
        showToast(rating ? 'Cảm ơn đánh giá tích cực của bạn!' : 'Cảm ơn phản hồi của bạn!');
    } catch (error) {
        console.error('Error rating response:', error);
    }
} 