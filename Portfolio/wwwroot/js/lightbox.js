function setLightboxImages(lightboxName)
{
    // Retrieve all the corresponding images of the lightbox
    let images = document.querySelectorAll(`img[data-bound-l-box="${lightboxName}"]`);
    _setIndices(images);
}

function _setIndices(images)
{
    let objref = DotNet.createJSObjectReference(window);
    images.forEach((image, i) => {
        image.dataset.lBoxIdx = i;
        console.log(i);
        DotNet.invokeMethod("Portfolio", "SetIndex", objref)
    })
    
    DotNet.disposeJSObjectReference(objref)
}