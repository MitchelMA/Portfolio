document.addEventListener('scroll', (e) => {
    showBtnToTop(document.querySelector('header.top-header').clientHeight);
})

window.addEventListener("resize", (e) => {
    checkTopNav();
})