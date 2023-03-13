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
    let elementPosition = element.getBoundingClientRect().top;
    let offsetPosition = elementPosition + window.scrollY - topOffset;
    
    document.documentElement.scrollTo({
        top: offsetPosition,
        behavior: "smooth"
    });
}