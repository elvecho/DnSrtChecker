var trxGlobal = {
    rtServerId: "",
    isChecked: "",
    isArchived: "",
    checkNote: "",
    DRtDeviceClosureDateTime: ""
};
$(document).ready(function () {

    var day = $("#date").val()
    var days = $(".panel-default");
    var index = 0;

    if (!isEmpty($("#date").val())) {
        for (var i = 0; i < days.length; i++) {
            if (day.includes(days[i].children[0].value)) {
                index = i;
            }
        }
        console.log(index)
        $(".bodyCollapseTrx").not(this).hide();
        console.log($(".panel-default")[index].children)
        $(".panel-default")[index].children[2].setAttribute("style", "display:");
        $(".panel-default")[index].children[3].setAttribute("style", "display:");
        $(".panel-default")[index].children[4].setAttribute("style", "display:");

        $(".panel-default")[index].scrollIntoView()
    }
    else {
        if ($(".clickableTrx").length == 1) {
            $(".bodyCollapseTrx").css("display", "");
        }
        else {
            $(".bodyCollapseTrx:first").siblings(".bodyCollapseTrx").css("display", "");
            $(".bodyCollapseTrx:first").css("display", "");

        }
    }



    var tableTrn = $(".transmissionTable");
    for (var i = 0; i < tableTrn.length; i++) {
        // console.log(tableTrn[i].getElementsByTagName("tbody")[0].children.length);
        if (tableTrn[i].getElementsByTagName("tbody")[0].children.length == 0) {
            var newRow = tableTrn[i].getElementsByTagName("tbody")[0].insertRow(0);
            newRow.innerHTML = "<tr><td colspan='9'>Nessuna trasmissione</td></tr>";
        }
    }



});
//Collapsable  table
//$('.clickableTrx').click(function () {
//   // $(".bd-example-modal-lg").modal("show");
//    var date = $(this).parent().parent()[0].children[0].value;
//    $(this).children()[1].classList.remove("hidden");
//    var res = getTrxDay(date);
//    $(this).parent().nextAll(".bodyCollapseTrx")[1].innerHTML = res.data;   
//    $(".bodyCollapseTrx").not(this).hide();
//    $(this).parent().nextAll(".bodyCollapseTrx").toggle();
//    $(this).parent().parent()[0].scrollIntoView();
//    var tableTrn = $(".transmissionTable");
//    for (var i = 0; i < tableTrn.length; i++) {
//        // console.log(tableTrn[i].getElementsByTagName("tbody")[0].children.length);
//        if (tableTrn[i].getElementsByTagName("tbody")[0].children.length == 0) {
//            var newRow = tableTrn[i].getElementsByTagName("tbody")[0].insertRow(0);
//            newRow.innerHTML = "<tr><td colspan='9'>Nessuna trasmissione</td></tr>";
//        }
//    }
   
//    //setTimeout(function () {
//    //    $('.bd-example-modal-lg').modal('hide');
//    //}, 4000);


    
//    console.log($(this).parent().parent())

//   // non funziona 
//   // $(".panel-heading").not(this).css("backgroundColor","Red")
//});

function tdclick(serverId, store, storeGroup, date) {
    window.location.href = "/Transactions/Filter?serverRt=" + serverId + "&store=" + store + "&storeGroup=" + storeGroup + "&TransactionDateTo=" + date + "&TransactionDateFrom=" + date;


}
//Check if string is empty
function isEmpty(s) {
    return !$.trim(s);
}


function LaunchModalLog(trn) {
    console.log(trn)
    console.log($(trn).siblings()[2].innerHTML)
    $("#textUserLog").html($(trn).siblings()[2].innerHTML)
}


//Ajax Call for saving checked flag 
function editIsCheckedTrxFlag(trn) {
    console.log(trn)
    editCheckTrx("check", trn);

}
//Ajax Call for saving archived flag 

function editIsArchivedTrxFlag(trn) {
    editArchiveTrx("archive", trn);
}



function LaunchModalNote(trx) {
    var value = $(trx).prev("#descriptionNoteCheckTrx")[0].value;
    console.log(value);
    $("#checkedNoteInputTrx").val(value);
    trxGlobal = getObjectJsonNoteEditTrx(trx);

}

//Call Ajax to save CheckNote 
function editCheckNoteTrx() {
    trxGlobal.checkNote = $("#checkedNoteInputTrx").val();
    console.log(trxGlobal);
    editNoteEditTrx("note", trxGlobal);
}

//Ajax Call for 
function editCheckTrx(changeOption, trn) {
    console.log(trn);
    var object = getObjectJsonTrx(trn);
    console.log(object)
    $.ajax({
        type: "GET",
        url: "/Transmissions/IsChecked",
        contentType: "application/json;charset=utf-8",
        data: { transmission: JSON.stringify(object) },
        dataType: "json",
        success: function (result) {
            manageTogglesTrx(object, changeOption, trn)
            location.reload();
        },
        error: function (result) {
            alert("Errore nell'eseguire le modifiche")
        }
    })
        ;
}
// Ajax call Archive
function editArchiveTrx(changeOption, trn) {
    var object = getObjectJsonTrx(trn);
    console.log(object)

    $.ajax({
        type: "GET",
        url: "/Transmissions/IsArchived",
        contentType: "application/json;charset=utf-8",
        data: { transmission: JSON.stringify(object) },
        dataType: "json",
        success: function (result) {
            manageTogglesTrx(object, changeOption, trn)
            location.reload();
        },
        error: function (result) {
            alert("Errore nell'eseguire le modifiche")
        }
    });
}

//function getTrxDay(date) {
//    var trxObject = {
//        id: $("#rtServerId").val(),
//        storeId: $("#storeId").val(),
//        storeGroupId: $("#storeGroupId").val(),
//        date:date,
//    };
//    var res = "";
//    $.ajax({
//        type: "GET",
//        url: "/Transmissions/GetTrxDay",
//        contentType: "application/json;charset=utf-8",
//        async:false,
//        data: { data: JSON.stringify(trxObject)},
//        dataType: "json",
//        success: function (result) {
//            res = result;
//        },
//        error: function (result) {
//            alert("Errore nell'eseguire le modifiche")
//        }
//    });
//    return res
//}


function editNoteEditTrx(changeOption, trn) {
    // var object = getObjectJsonNoteEditTrx(trn);
    var object = trxGlobal;
    console.log(object)
    $.ajax({
        type: "GET",
        url: "/Transmissions/NoteEdit",
        contentType: "application/json;charset=utf-8",
        data: { transmission: JSON.stringify(object) },
        dataType: "json",
        success: function (result) {
            manageTogglesTrx(object, changeOption)
            //location.reload();
        },
        error: function (result) {
            alert("Errore nell'eseguire le modifiche")
        }
    });
}

function getObjectJsonNoteEditTrx(trn) {
    var id = trn.id;
    // console.log(id)
    //console.log($(trn).parents()[6].children[0].value)
    var date = $(trn).parents()[5].children[0].value;
    console.log(date)
    var trxObject = {
        rtServerId: $("#rtServerId").val(),
        isChecked: $(trn).is(":checked"),
        isArchived: $(trn).is(":checked"),
        checkNote: $(trn).val(),
        DRtDeviceClosureDateTime: date// $("#OperationClosureDatetimeHidden").val()
    };

    return trxObject;
}
//Prepare JsonObject for transaction's operations
function getObjectJsonTrx(trn) {
    var id = trn.id;
    //console.log($(trn).parents()[6].children[0])
    // var date = $(trn).parents()[3].children[0].children[0].children[0].value
    var date = $(trn).parents()[6].children[0].value;
    console.log(date)
    //console.log($(trn).parents()[3].children[0].children[0].children[0].value)
    var trxObject = {
        rtServerId: $("#rtServerId").val(),
        isChecked: $(trn).is(":checked"),
        isArchived: $(trn).is(":checked"),
        checkNote: $(trn).val(),
        DRtDeviceClosureDateTime: date// $("#OperationClosureDatetimeHidden").val()
    };

    return trxObject;
}


function manageTogglesTrx(object, changeOption, trn) {
    //location.reload();
    console.log(trn)
    if (changeOption == "check") {
        if (object.isChecked == true) {
            $("#resultCheckTrx").removeClass("label-dangerColor");
            $("#resultCheckTrx").addClass("label-successColor");
            $("#resultCheckTrx").text("Transmissione verificata con successo");
        }
        else {
            $("#resultCheckTrx").removeClass("label-successColor");
            $("#resultCheckTrx").addClass("label-dangerColor");
            $("#resultCheckTrx").text("Transmissione non verificata");
        }
        $("#resultCheckTrx").show().delay(2000).fadeOut();
    }
    if (changeOption == "archive") {
        if (object.isArchived == true) {
            $("#resultArchiveTrx").removeClass("label-dangerColor");
            $("#resultArchiveTrx").addClass("label-successColor");
            $("#resultArchiveTrx").text("Transmissione archiviata con successo");
        }
        else {
            $("#resultArchiveTrx").removeClass("label-successColor");
            $("#resultArchiveTrx").addClass("label-dangerColor");
            $("#resultArchiveTrx").text("Transmissione non archiviata");

        }
        $("#resultArchiveTrx").show().delay(2000).fadeOut();
    }
    if (changeOption == "note") {
        location.reload();
        // $("#checkNote").modal('hide');
    }

}


//test
function getTrxDay(date,handleData) {
    var trxObject = {
        id: $("#rtServerId").val(),
        storeId: $("#storeId").val(),
        storeGroupId: $("#storeGroupId").val(),
        date: date,
    };
    var res = "";
    $.ajax({
        type: "GET",
        url: "/Transmissions/GetTrxDay",
        contentType: "application/json;charset=utf-8",
        //async: false,
        data: { data: JSON.stringify(trxObject) },
        dataType: "json",
        success: function (result) {
            handleData(result.data);
            res = result;
        },
        error: function (result) {
            alert("Errore nell'eseguire le modifiche")
        }
    });
   // return res
}
// Old onclick before 03 september 2020

//$('.clickableTrx').click(function () {
//    // $(".bd-example-modal-lg").modal("show");
//    $(this).find(".spinner").classList.remove("hidden");
////    $(this).children()[1].classList.remove("hidden");
//    var date = $(this).parent().parent()[0].children[0].value;
//    var clickededItem = $(this);
//    getTrxDay(date, function (handleData) {
//        console.log(handleData);
//        clickededItem.parent().nextAll(".bodyCollapseTrx")[1].innerHTML = handleData.data;
//        $(".bodyCollapseTrx").not(clickededItem).hide();
//        clickededItem.parent().nextAll(".bodyCollapseTrx").toggle();
//        clickededItem.parent().parent()[0].scrollIntoView();
//        var tableTrn = $(".transmissionTable");
//        for (var i = 0; i < tableTrn.length; i++) {
//            // console.log(tableTrn[i].getElementsByTagName("tbody")[0].children.length);
//            if (tableTrn[i].getElementsByTagName("tbody")[0].children.length == 0) {
//                var newRow = tableTrn[i].getElementsByTagName("tbody")[0].insertRow(0);
//                newRow.innerHTML = "<tr><td colspan='9'>Nessuna trasmissione</td></tr>";
//            }
//        }
//        $(this).find(".spinner").classList.remove("hidden");

//      //  clickededItem.children()[1].classList.add("hidden");

//    });
   
//});

// 03 september 2020 edit onclick
$('.clickableTrx').click(function () {
    // $(".bd-example-modal-lg").modal("show");
    //    $(this).children()[1].classList.remove("hidden");
    var date = $(this).parent().parent()[0].children[0].value;
    var clickededItem = $(this);
    clickededItem.children(".spinner")[0].classList.remove("hidden");

    getTrxDay(date, function (handleData) {

        console.log(handleData);
        clickededItem.parent().nextAll(".bodyCollapseTrx")[1].innerHTML = handleData;
        $(".bodyCollapseTrx").not(clickededItem).hide();
        clickededItem.parent().nextAll(".bodyCollapseTrx").toggle();
        clickededItem.parent().parent()[0].scrollIntoView();
        var tableTrn = $(".transmissionTable");
        for (var i = 0; i < tableTrn.length; i++) {
            // console.log(tableTrn[i].getElementsByTagName("tbody")[0].children.length);
            if (tableTrn[i].getElementsByTagName("tbody")[0].children.length == 0) {
                var newRow = tableTrn[i].getElementsByTagName("tbody")[0].insertRow(0);
                newRow.innerHTML = "<tr><td colspan='9'>Nessuna trasmissione</td></tr>";
            }
        }
        //  clickededItem.children()[1].classList.add("hidden");
        clickededItem.children(".spinner")[0].classList.add("hidden");

    });
});
