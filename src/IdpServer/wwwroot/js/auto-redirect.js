$(window).on('load',function () {
  var timeleft = document.getElementById("countdown").innerHTML;
  var downloadTimer = setInterval(function () {
    if (timeleft <= 0) {
      clearInterval(downloadTimer);
      $('#btn-ok').trigger('click');
    } else {
      document.getElementById("countdown").innerHTML = timeleft;
    }
    timeleft -= 1;
  }, 1000);
});