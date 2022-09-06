
function redirectToStore() {
    var object = createRtObject();
   /* $.ajax({
        type: "GET",
        url: "/RtServers/RedirectToStore",
        contentType: "application/json;charset=utf-8",
        data: { 'rtServer': JSON.stringify(object) },
        dataType: "json",
        success:console.log("Ok")
    });*/
    window.location.href = "/RtServers/RedirectToStore?rtServer=" + JSON.stringify(object);
    event.preventDefault();
}
function createRtObject() {
    var rtServer = {
        SzRtServerId: $("input[name='SzRtServerId']").val(),
        SzLocation: $("input[name='SzLocation']").val(),
        SzIpAddress: $("input[name='SzIpAddress']").val(),
        SzUsername: $("input[name='SzUsername']").val(),
        SzPassword: $("input[name='SzPassword']").val(),
        BOnDutyFlag: $("input[name='BOnDutyFlag']").val()
    }
    console.log(rtServer)
    return rtServer;
}