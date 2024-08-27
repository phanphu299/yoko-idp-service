function changeImage(im) {
    var video = document.getElementById("video-background");
    var source = document.getElementById("source-video-background");
    source.src = im;
    source.currentSrc = im;
    video.load();
}
var sizeScreen = screen.width;
if (sizeScreen <= 375) {
    changeImage('https://cdn.ahi.apps.yokogawa.com/assets/video/bg_idp/mobile.mp4');
} else if (sizeScreen <= 1024 && sizeScreen > 375) {
    changeImage('https://cdn.ahi.apps.yokogawa.com/assets/video/bg_idp/tablet.mp4');
} else {
    changeImage('https://cdn.ahi.apps.yokogawa.com/assets/video/bg_idp/PC.mp4');
}
