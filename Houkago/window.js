document.body.style.transformOrigin = "left top";
document.body.style.overflow = "hidden";
const gameFrame = document.getElementById("game_contents");
document.body.insertBefore(gameFrame, document.body.firstChild);
//gameFrame.style.position = "fixed";
gameFrame.style.top = "0px";
gameFrame.style.left = "0px";
gameFrame.style.zIndex = "99";
window.addEventListener('resize', onWindowResize, false);
window.addEventListener('focus', onWindowResize, false);
function onWindowResize() {
    let widthScale = window.innerWidth / 960;
    let heightScale = window.innerHeight / 540;
    if (widthScale < heightScale) {
        document.body.style.transform = "scale(" + widthScale + ")";
    } else {
        document.body.style.transform = "scale(" + heightScale + ")";
    }
    window.scrollTo(0, 0)
}
onWindowResize();