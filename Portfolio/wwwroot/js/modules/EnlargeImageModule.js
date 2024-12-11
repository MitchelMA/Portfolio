/**
 * 
 * @param query querystring used to search for the images to add the handlers to
 */
export function addImageHandlers(query) {
    const images = document.querySelectorAll(query);
    console.log(images);
    images.forEach(image => {
        // image.addEventListener("click", imageClickHandlerBuilder(image));
        image.onclick = imageClickHandlerBuilder(image);
    });
    return images.length;
}

export function getScreenSize() {
    return [window.innerWidth, window.innerHeight];
}

/**
 * 
 * @param image
 * @returns {(function(*))|*}
 */
function imageClickHandlerBuilder(image) {
    return async (e) => {
        const clientRect = image.getBoundingClientRect();
        clientRect.height
        const intercepted = await DotNet.invokeMethodAsync("Portfolio", "EnlargeImage",
            image.src, image.alt, clientRect.x, clientRect.y, clientRect.width, clientRect.height);
        
        if (intercepted)
            return;
        
        image.setAttribute("opened", true);
    };
}