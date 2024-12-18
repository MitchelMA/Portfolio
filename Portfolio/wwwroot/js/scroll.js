function scrollToTop()
{
    document.documentElement.scrollIntoView({behavior: "smooth", block: "start", inline: "nearest"})
}

function scrollToId(id, topOffset = 50)
{
    let elem = document.getElementById(id);
    if(!elem)
        return;
    
    scrollToElem(elem, topOffset);
}

function scrollToElem(element, topOffset)
{
    const navHeight = document.querySelector("nav.topnav")?.getBoundingClientRect().height ?? 0;
    const elementRect = element.getBoundingClientRect();
    const elementPosition = elementRect.top;
    const offsetPosition = elementPosition + window.scrollY - navHeight - topOffset;
    
    document.documentElement.scrollTo({
        top: offsetPosition,
        behavior: "smooth"
    });
}