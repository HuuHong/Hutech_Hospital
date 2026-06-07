/**
 * HUTECH_HOSPITAL PREMIUM MEDICAL UI ANIMATIONS
 * Handles smooth scrolling, scroll reveals, parallax, and UI interactions.
 */

document.addEventListener("DOMContentLoaded", () => {
    // Add js-enabled to body so CSS knows it can safely hide elements for animation
    document.body.classList.add('js-enabled');
    
    // 1. Intersection Observer for Scroll Animations (Reveal Elements)
    const revealElements = document.querySelectorAll('.reveal, .reveal-up, .reveal-left, .reveal-right');
    
    if (revealElements.length > 0 && 'IntersectionObserver' in window) {
        const revealOptions = {
            threshold: 0.1,
            rootMargin: "0px 0px -50px 0px"
        };
        
        const revealObserver = new IntersectionObserver((entries, observer) => {
            entries.forEach(entry => {
                if (entry.isIntersecting) {
                    entry.target.classList.add('active');
                    // Optional: Unobserve after revealing to only animate once
                    // observer.unobserve(entry.target); 
                }
            });
        }, revealOptions);
        
        revealElements.forEach(el => revealObserver.observe(el));
    } else {
        // Fallback for older browsers
        revealElements.forEach(el => el.classList.add('active'));
    }

    // 2. Navbar Glassmorphism on Scroll
    const navbar = document.querySelector('.medical-navbar');
    if (navbar) {
        window.addEventListener('scroll', () => {
            if (window.scrollY > 50) {
                navbar.style.background = 'rgba(255, 255, 255, 0.95)';
                navbar.style.boxShadow = 'var(--medical-shadow)';
            } else {
                navbar.style.background = 'rgba(255, 255, 255, 0.7)';
                navbar.style.boxShadow = 'var(--medical-shadow-sm)';
            }
        });
    }

    // 3. Smooth Scroll for Anchor Links
    document.querySelectorAll('a[href^="#"]').forEach(anchor => {
        anchor.addEventListener('click', function(e) {
            const targetId = this.getAttribute('href');
            if (targetId === '#') return;
            
            const targetElement = document.querySelector(targetId);
            if (targetElement) {
                e.preventDefault();
                const headerOffset = 80;
                const elementPosition = targetElement.getBoundingClientRect().top;
                const offsetPosition = elementPosition + window.pageYOffset - headerOffset;
                
                window.scrollTo({
                    top: offsetPosition,
                    behavior: "smooth"
                });
            }
        });
    });

    // 4. Parallax Effect for Hero Orbs
    const heroSection = document.querySelector('.hospital-hero');
    const orbs = document.querySelectorAll('.orb');
    
    if (heroSection && orbs.length > 0) {
        document.addEventListener('mousemove', (e) => {
            const x = e.clientX / window.innerWidth;
            const y = e.clientY / window.innerHeight;
            
            orbs.forEach((orb, index) => {
                const speed = (index + 1) * 20;
                const moveX = (x * speed) - (speed/2);
                const moveY = (y * speed) - (speed/2);
                orb.style.transform = `translate(${moveX}px, ${moveY}px)`;
            });
        });
    }

    // 5. Quick Booking Form Submit with SweetAlert2
    const quickBookingForm = document.getElementById('quickBookingForm');
    if (quickBookingForm) {
        quickBookingForm.addEventListener('submit', function(e) {
            e.preventDefault(); // Prevent actual submission to demo SweetAlert
            
            const submitBtn = this.querySelector('button[type="submit"]');
            const originalText = submitBtn.innerHTML;
            
            // Loading state
            submitBtn.innerHTML = '<i class="fa-solid fa-spinner fa-spin"></i> Đang xử lý...';
            submitBtn.disabled = true;
            
            // Simulate network request
            setTimeout(() => {
                submitBtn.innerHTML = originalText;
                submitBtn.disabled = false;
                
                // Show SweetAlert
                if (typeof Swal !== 'undefined') {
                    Swal.fire({
                        title: 'Thành công!',
                        text: 'Yêu cầu đặt lịch đã được ghi nhận. Vui lòng đăng nhập để hoàn tất đặt lịch.',
                        icon: 'success',
                        confirmButtonColor: '#0ea5e9',
                        confirmButtonText: 'Đăng nhập ngay'
                    }).then((result) => {
                        if (result.isConfirmed) {
                            window.location.href = '/Account/Login';
                        }
                    });
                } else {
                    alert('Yêu cầu đặt lịch đã được ghi nhận. Vui lòng đăng nhập để hoàn tất đặt lịch.');
                }
                
                this.reset(); // Reset form
            }, 1500);
        });
    }

    // 6. Basic Search functionality
    const searchInput = document.getElementById('globalSearch');
    if (searchInput) {
        searchInput.addEventListener('keypress', function(e) {
            if (e.key === 'Enter') {
                e.preventDefault();
                const keyword = this.value.trim();
                if (keyword) {
                    // Navigate to Doctor search with query string
                    window.location.href = `/Doctor?keyword=${encodeURIComponent(keyword)}`;
                }
            }
        });
    }
});
