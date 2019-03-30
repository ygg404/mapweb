$(function () {
    //if (!isWeiXin()) {
    //    window.location.href = contentPath + "ErrorWxBrowser";
    //}
})
isWeiXin=function isWeiXin() {
    var ua = window.navigator.userAgent.toLowerCase();
    if (ua.match(/MicroMessenger/i) == 'micromessenger') {
        return true;
    } else {
        return false;
    }
}