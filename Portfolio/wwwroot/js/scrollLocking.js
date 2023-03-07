let body = document.body;

function lockScroll(lockClassName) {
    body.classList.add(lockClassName);
}

function unlockScroll(lockClassName) {
    body.classList.remove(lockClassName);
}