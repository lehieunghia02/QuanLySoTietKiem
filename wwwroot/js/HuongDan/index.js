document.addEventListener('DOMContentLoaded', function () {
    // Thêm hiệu ứng xuất hiện dần cho các phần tử
    const guideCards = document.querySelectorAll('.guide-card');
    guideCards.forEach((card, index) => {
        card.style.animationDelay = `${0.1 * (index + 1)}s`;
    });

    // Thêm hiệu ứng hover cho các mục hướng dẫn
    const guideSections = document.querySelectorAll('.guide-section');
    guideSections.forEach(section => {
        section.addEventListener('mouseenter', function () {
            this.style.transform = 'translateX(10px)';
        });

        section.addEventListener('mouseleave', function () {
            this.style.transform = '';
        });
    });
});