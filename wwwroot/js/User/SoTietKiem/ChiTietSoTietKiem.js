document.addEventListener('DOMContentLoaded', function () {
                // Thêm hiệu ứng hover cho các phần tử
                const infoItems = document.querySelectorAll('.info-item');
                infoItems.forEach(item => {
                  item.addEventListener('mouseenter', function () {
                    this.style.transform = 'translateY(-5px) scale(1.02)';
                  });

                  item.addEventListener('mouseleave', function () {
                    this.style.transform = '';
                  });
                });
              });