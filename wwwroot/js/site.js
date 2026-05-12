/* ============================================================
   SHADI HALL — site.js
   Auto-slider, navbar scroll, hamburger menu, misc UI
============================================================ */

document.addEventListener('DOMContentLoaded', function () {

  // ── Navbar scroll effect ──────────────────────────────────
  const navbar = document.getElementById('mainNav');
  if (navbar) {
    window.addEventListener('scroll', function () {
      navbar.classList.toggle('scrolled', window.scrollY > 40);
    });
  }

  // ── Hamburger menu ────────────────────────────────────────
  const hamburger = document.getElementById('hamburger');
  const navLinks  = document.getElementById('navLinks');
  if (hamburger && navLinks) {
    hamburger.addEventListener('click', function () {
      const open = navLinks.classList.toggle('open');
      hamburger.setAttribute('aria-expanded', open);
      // animate bars
      const bars = hamburger.querySelectorAll('span');
      if (open) {
        bars[0].style.transform = 'translateY(7px) rotate(45deg)';
        bars[1].style.opacity   = '0';
        bars[2].style.transform = 'translateY(-7px) rotate(-45deg)';
      } else {
        bars.forEach(b => { b.style.transform = ''; b.style.opacity = ''; });
      }
    });
    // Close on outside click
    document.addEventListener('click', function (e) {
      if (!navbar.contains(e.target) && navLinks.classList.contains('open')) {
        navLinks.classList.remove('open');
        hamburger.querySelectorAll('span').forEach(b => { b.style.transform = ''; b.style.opacity = ''; });
      }
    });
  }

  // ── Hero Slider ───────────────────────────────────────────
  const sliderWrapper = document.getElementById('slidesWrapper');
  const dots          = document.querySelectorAll('.dot');
  const prevBtn       = document.getElementById('prevBtn');
  const nextBtn       = document.getElementById('nextBtn');

  if (sliderWrapper) {
    const slides     = sliderWrapper.querySelectorAll('.slide');
    const total      = slides.length;
    let   current    = 0;
    let   autoTimer  = null;

    function goTo(index) {
      current = (index + total) % total;
      sliderWrapper.style.transform = `translateX(-${current * 100}%)`;
      dots.forEach((d, i) => d.classList.toggle('active', i === current));
    }

    function startAuto() {
      stopAuto();
      autoTimer = setInterval(() => goTo(current + 1), 5000);
    }

    function stopAuto() {
      if (autoTimer) clearInterval(autoTimer);
    }

    if (total > 1) {
      if (prevBtn) prevBtn.addEventListener('click', () => { goTo(current - 1); startAuto(); });
      if (nextBtn) nextBtn.addEventListener('click', () => { goTo(current + 1); startAuto(); });
      dots.forEach((d, i) => d.addEventListener('click', () => { goTo(i); startAuto(); }));

      // Pause on hover
      sliderWrapper.closest('.hero-slider')?.addEventListener('mouseenter', stopAuto);
      sliderWrapper.closest('.hero-slider')?.addEventListener('mouseleave', startAuto);

      // Touch swipe
      let touchStartX = 0;
      sliderWrapper.addEventListener('touchstart', e => { touchStartX = e.touches[0].clientX; }, { passive: true });
      sliderWrapper.addEventListener('touchend', e => {
        const diff = touchStartX - e.changedTouches[0].clientX;
        if (Math.abs(diff) > 50) { goTo(current + (diff > 0 ? 1 : -1)); startAuto(); }
      });

      startAuto();
    }
  }

  // ── Alert auto-dismiss ────────────────────────────────────
  document.querySelectorAll('.alert-dismissible').forEach(alert => {
    setTimeout(() => {
      alert.style.transition = 'opacity .4s, max-height .4s';
      alert.style.opacity    = '0';
      alert.style.maxHeight  = '0';
      alert.style.overflow   = 'hidden';
      setTimeout(() => alert.remove(), 400);
    }, 5000);
  });

  // ── Smooth scroll for dashboard anchor links ──────────────
  document.querySelectorAll('a[href^="#"]').forEach(link => {
    link.addEventListener('click', function (e) {
      const target = document.querySelector(this.getAttribute('href'));
      if (target) {
        e.preventDefault();
        target.scrollIntoView({ behavior: 'smooth', block: 'start' });
        // Update active nav
        document.querySelectorAll('.dash-nav-link').forEach(l => l.classList.remove('active'));
        this.classList.add('active');
      }
    });
  });

  // ── Confirm before form submits (data-confirm) ────────────
  document.querySelectorAll('[data-confirm]').forEach(el => {
    el.addEventListener('click', function (e) {
      if (!confirm(this.dataset.confirm)) e.preventDefault();
    });
  });

  // ── Image preview on file input ───────────────────────────
  document.querySelectorAll('input[type="file"][accept="image/*"]').forEach(input => {
    input.addEventListener('change', function () {
      const preview = this.closest('.form-group')?.querySelector('.preview-img');
      if (preview && this.files && this.files[0]) {
        const reader = new FileReader();
        reader.onload = e => { preview.src = e.target.result; preview.style.display = 'block'; };
        reader.readAsDataURL(this.files[0]);
      }
    });
  });

  // ── Form input animations ─────────────────────────────────
  document.querySelectorAll('.form-input').forEach(input => {
    input.addEventListener('focus', function () {
      this.closest('.form-group')?.classList.add('focused');
    });
    input.addEventListener('blur', function () {
      this.closest('.form-group')?.classList.remove('focused');
    });
  });

  // ── Counter animation for stats ───────────────────────────
  function animateCounters() {
    document.querySelectorAll('.stat-number, .admin-stat-num, .dash-stat-num').forEach(el => {
      const text = el.innerText;
      const num  = parseFloat(text.replace(/[^0-9.]/g, ''));
      if (isNaN(num) || num === 0) return;
      const prefix = text.includes('৳') ? '৳' : '';
      const suffix = text.includes('+') ? '+' : '';
      const decimals = text.includes('.') ? 1 : 0;
      let start = 0;
      const duration = 1200;
      const step = 16;
      const increment = num / (duration / step);
      const timer = setInterval(() => {
        start = Math.min(start + increment, num);
        el.innerText = prefix + (decimals ? start.toFixed(1) : Math.floor(start).toLocaleString()) + suffix;
        if (start >= num) clearInterval(timer);
      }, step);
    });
  }

  // Trigger counter on intersection
  const statsObserver = new IntersectionObserver(entries => {
    entries.forEach(entry => {
      if (entry.isIntersecting) {
        animateCounters();
        statsObserver.disconnect();
      }
    });
  }, { threshold: 0.4 });

  const statsEl = document.querySelector('.stats-bar, .admin-stats-grid, .dash-stats');
  if (statsEl) statsObserver.observe(statsEl);

  // ── Fade-in on scroll (hall cards, review cards) ──────────
  const fadeEls = document.querySelectorAll('.hall-card, .review-card, .admin-stat-card, .dash-stat-card');
  if (fadeEls.length && 'IntersectionObserver' in window) {
    fadeEls.forEach(el => {
      el.style.opacity  = '0';
      el.style.transform = 'translateY(20px)';
      el.style.transition = 'opacity .5s ease, transform .5s ease';
    });
    const cardObserver = new IntersectionObserver(entries => {
      entries.forEach((entry, i) => {
        if (entry.isIntersecting) {
          setTimeout(() => {
            entry.target.style.opacity   = '1';
            entry.target.style.transform = 'translateY(0)';
          }, (i % 4) * 80);
          cardObserver.unobserve(entry.target);
        }
      });
    }, { threshold: 0.1 });
    fadeEls.forEach(el => cardObserver.observe(el));
  }

  // ── Min date enforcement on date inputs ───────────────────
  document.querySelectorAll('input[type="date"]').forEach(input => {
    if (!input.min) {
      const tomorrow = new Date();
      tomorrow.setDate(tomorrow.getDate() + 1);
      input.min = tomorrow.toISOString().split('T')[0];
    }
  });

  // ── Table row highlight ───────────────────────────────────
  document.querySelectorAll('.data-table tbody tr').forEach(row => {
    row.style.transition = 'background .15s ease';
  });

});
