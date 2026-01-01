// Animation for stats numbers
document.addEventListener('DOMContentLoaded', function () {
    // Animate stats when they come into view
    const stats = document.querySelectorAll('.stat-number');

    const observer = new IntersectionObserver((entries) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                const target = entry.target;
                const count = parseInt(target.getAttribute('data-count'));

                // If it's a decimal number
                if (target.getAttribute('data-count').includes('.')) {
                    const countDecimal = parseFloat(target.getAttribute('data-count'));
                    let startCount = 0;
                    const duration = 2000;
                    const step = 30;
                    const increment = countDecimal / (duration / step);

                    const timer = setInterval(() => {
                        startCount += increment;
                        if (startCount >= countDecimal) {
                            clearInterval(timer);
                            target.textContent = countDecimal.toFixed(1) + '/5';
                        } else {
                            target.textContent = startCount.toFixed(1) + '/5';
                        }
                    }, step);
                } else {
                    // For regular numbers
                    let startCount = 0;
                    const duration = 2000;
                    const step = 30;
                    const increment = count / (duration / step);

                    const timer = setInterval(() => {
                        startCount += increment;
                        if (startCount >= count) {
                            clearInterval(timer);
                            target.textContent = count.toLocaleString() + '+';
                        } else {
                            target.textContent = Math.floor(startCount).toLocaleString() + '+';
                        }
                    }, step);
                }

                observer.unobserve(target);
            }
        });
    }, {threshold: 0.5});

    stats.forEach(stat => {
        observer.observe(stat);
    });
});