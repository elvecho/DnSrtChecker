// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
//$('#logCheck').popover({
//    html: true,
//    content: function () {
//        console.log($('#popover-content-checked').html())
//        return $('#popover-content-checked').html();
//    }
//});
//$('#archived').popover({
//    html: true,
//    content: function () {
//        return $('#popover-content-archived').html();
//    }
//});
function displayBusyIndicator() {
    document.getElementById("loading").style.display = "block";
}