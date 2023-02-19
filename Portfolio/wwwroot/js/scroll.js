
document.addEventListener('scroll', (e) => {
    showBtnToTop(document.querySelector('header.top-header').clientHeight);
})

function scrollToTop()
{
    document.documentElement.scrollIntoView({behavior: "smooth", block: "start", inline: "nearest"})
}