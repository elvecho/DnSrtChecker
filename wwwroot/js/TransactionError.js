
function editCall() {
    $.ajax({
        type: "GET",
        url: "/TransactionError/Index",
        contentType: "application/json;charset=utf-8",
        data: { transaction: JSON.stringify(getObjectJson()) },
        dataType: "json",
        success: (result) => {
            console.log(result)
            location.reload();
        }
    });
}

function getObjectJson() {
    console.log($("#idTransaction").html())
    var trnObject = {
        idTransaction: $("#idTransaction").html(),
        rtServerId: $("#rtServerId").html(),
        retailStoreId: $("#retailStoreId").html(),
        storeGroupId: $("#storeGroupId").html(),
        isChecked: $("input[name=bTransactionCheckedFlag]").is(":checked"),
        isArchived: $("input[name=bTransactionArchivedFlag]").is(":checked"),
        checkNote: $("#checkedNoteInput").val()
    };
    return trnObject;
}