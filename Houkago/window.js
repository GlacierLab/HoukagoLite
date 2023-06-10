document.body.style.transformOrigin = "left top";
document.body.style.overflow = "hidden";
const gameFrame = document.getElementById("game_contents");
document.body.insertBefore(gameFrame, document.body.firstChild);
document.body.removeChild(document.getElementsByClassName("bg")[0]);
//gameFrame.style.position = "fixed";
gameFrame.style.zIndex = "99";
window.addEventListener('resize', onWindowResize, false);
function onWindowResize() {
    let widthScale = window.innerWidth / 960;
    let heightScale = window.innerHeight / 540;
    if (widthScale < heightScale) {
        document.body.style.transform = "scale(" + widthScale + ")";
        gameFrame.style.marginTop = ((window.innerHeight / widthScale) - 540)/2+"px";
        gameFrame.style.marginLeft = "0px";
    } else {
        document.body.style.transform = "scale(" + heightScale + ")";
        gameFrame.style.marginTop = "0px";
        gameFrame.style.marginLeft = ((window.innerWidth / heightScale) - 960) / 2 +"px";
    }
    window.scrollTo(0, 0)
}
onWindowResize();