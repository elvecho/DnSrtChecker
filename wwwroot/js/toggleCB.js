
function changeStatusServer(server) {
    var rtServer=getJsonObject(server)

    $.ajax({
        type: "GET",
        url: "/RtServers/StatusServerChange",
        contentType: "application/json;charset=utf-8",
        data: { 'param': JSON.stringify(rtServer) },
        dataType: "json",
        success: function(result) {
            console.log(result.statusServer)
           location.reload();

            if (!result.statusServer) {
                alert("Disabilitare prima il server dal vecchio Punto di vendita.")
            }
        }  
    }); 
}

function getJsonObject(server) {
    var rtServer = {
        id: server.id,
        retailStoreId: $(server).parent().parent().parent()[0].children[0].innerHTML,
        storeGroupId: $(server).parent().parent().parent()[0].children[1].innerHTML,
        statusSRV: $(server).is(":checked")
    }
    return rtServer
}