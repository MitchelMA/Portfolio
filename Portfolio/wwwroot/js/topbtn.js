function showBtnToTop(threshold) {
    let btn = document.querySelector("#toTopBtn");
    let fromTop = document.documentElement.scrollTop;

    if (fromTop < threshold) {
        btn.classList.remove("shown");
        return;
    }

    btn.classList.add("shown");
}