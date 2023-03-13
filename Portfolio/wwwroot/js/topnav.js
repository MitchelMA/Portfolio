function checkTopNav()
{
    let topnav = document.querySelector("nav.topnav");
    let nonStackedSize = Number(topnav.dataset.nonStackedSize);
    let windowWidth = window.innerWidth;
    
    if(windowWidth > nonStackedSize)
    {
        topnav.classList.add("non-stacked");
    }
    else {
        topnav.classList.remove("non-stacked");
    }
}
