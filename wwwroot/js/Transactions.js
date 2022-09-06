
var trInfo = {
    idTransaction: "",
    rtServerId: "",
    retailStoreId: "",
    storeGroupId: "",
    isChecked:"",
    isArchived: "",
    checkNote: "",
    transactionDate:""
};
var textRT = "";
var textTP = "";
var textLog = "";

$(document).ready(function () {
$('#logCheck').popover({
        html: true,
        content: function () {
            return $('#popover-content-checked').html();
        }
});
      $('#archived').popover({
        html: true,
        content: function () {
            return $('#popover-content-archived').html();
        }
    });
    if ($(".clickable").length == 1) {
        $(".bodyCollapse").removeAttr("style");
    }

    $("#transactionDateFrom").datepicker({
        setDate: new Date(),
        pickTime: false,
        dateFormat: "dd-mm-yy"
    });
    $("#transactionDateTo").datepicker({
        setDate: new Date(),
        pickTime: false,
        dateFormat: "dd-mm-yy"
    });

    $('.fa-calendar').click(function () {
        $("#transactionDate").focus();
    });

    $("#checkedNoteInput").val($("#descriptionNote").html());

    //documents manage
    if (isEmpty($("#textAreaDocTP").text())) {
        $("#tpDoc").attr("disabled", "disabled");
    }

    //if (isEmpty($("#textAreaDocRT").val())) {
    if (isEmpty($("#textAreaDocRT").text())) {
        $("#rtDoc").attr("disabled", "disabled");

    }

    if (isEmpty($("#textUserLog").text())) {
        $("#logModalBtn").attr("disabled", "disabled");

    }
    textRT = $("#textAreaDocRT").text();
    textTP = $("#textAreaDocTP").text();
  
});

//function showModalLog() {
//    console.log($("#textLog").html())
//    console.log($("#textLog").text())

//    $("#textLog").html(textLog);
//}
//upload and choice which document will be displayed
function showModalDocument(doc) {

    if (doc.id == "rtDoc") {
        $("#documentModalLabel").text("Scontrino Server RT");
        $("#documentBodyTP").attr("style", "display:none");

        $("#textAreaDocRT").html(textRT);

        $("#documentBodyRT").attr("style", "display:''");
       

    }
    else {
        $("#documentModalLabel").text("Scontrino Cassa");
        var doc = "";
        var t = textTP.split("\n");
        t.forEach(function (s) {
            if (s.includes("{BOLD}")) {
                s = s.replace("{BOLD}", "<b>");
                s = s.concat( "</b>");
                s = s.replace("\\x","")
            }
            if (s.includes("{Bar")) {
                var endIndex = s.lastIndexOf("}");
                s = s.substring(endIndex + 1).bold();
                s = s.replace("\\x", "");
            }
            if (s.includes("{FONTA,CENTER}")) {
                s = s.replace("\\x{FONTA,CENTER}", "<center>");
                s = s.concat("</center>")
                s = s.replace("\\x", "")
            }
            doc = doc.concat(s);
        });

        $("#documentBodyTP").css("display", "");
        $("#documentBodyRT").attr("style", "display:none");
        $("#textAreaDocTP").html(doc);


    }
}

//Collapsable rows table
$('.clickable').click(function () {
    $(".bodyCollapse").not(this).hide();
    $(this).parent().next('tbody').toggle();
});

//Action for Erase filters
/*$("#btnEraser").click(function(e) {
    var d = $("#transactionDate").val();
    console.log(d)
    if (d.length == 0) {
        $(this).popover({
            title: 'Attenzione!!',
            content: 'Devi prima scegliere una data.',
            placement: 'right',
            trigger: "manual"
        }).popover('show');
        e.preventDefault();
    }
});*/

//Autoselect Verificata when Conformi e senza differenza will be selected
$("#NonCompliantOrHasMismatch").change(function () {
    if (this.value == "false") {
        $("#isChecked").val("");
    }
});

//Ajax Call for saving checked flag 
function editIsCheckedFlag(trn) {
    editCheck("check");

}
//Ajax Call for saving archived flag 

function editIsArchivedFlag(trn) {
    editArchive("archive");
}


//Prepare JsonObject for transaction's operations
function getObjectJson() {
    console.log($("#idTransaction").val())
    var trnObject = {
        idTransaction: $("#idTransaction").html(),
        rtServerId: $("#rtServerId").html(),
        retailStoreId: $("#retailStoreId").html(),
        storeGroupId: $("#storeGroupId").html(),
        isChecked: $("input[name=bTransactionCheckedFlag]").is(":checked"),
        isArchived:$("input[name=bTransactionArchivedFlag]").is(":checked"),
        checkNote: $("#checkedNoteInput").val(),
        transactionDateFrom: $("#transactionDateFrom").val(),
        transactionDateTo: $("#transactionDateTo").val()
    };
    

    return trnObject;
}

//Check if string is empty
function isEmpty(s) {
    return !$.trim(s);
    //return !s.length;
}

//Call Ajax to save CheckNote 
$("#saveCheckNote").click(function () {
    editNoteEdit("note");
});

//Ajax Call for 
function editCheck(changeOption) {
    var object = getObjectJson();
    console.log(object)

    $.ajax({
        type: "GET",
        url: "/Transactions/IsChecked",
        contentType: "application/json;charset=utf-8",
        data: { transaction: JSON.stringify(object) },
        dataType: "json",
        success: function (result) {
            manageToggles(object, changeOption)
            location.reload();
        },
        error: function (result) {
            alert("Errore nell'eseguire le modifiche")
        }
        
    })
        ;
}
// Ajax call Archive
function editArchive(changeOption) {
    var object = getObjectJson();
    console.log(object)

    $.ajax({
        type: "GET",
        url: "/Transactions/IsArchived",
        contentType: "application/json;charset=utf-8",
        data: { transaction: JSON.stringify(object) },
        dataType: "json",
        success: function (result) {
            manageToggles(object, changeOption)
            location.reload();
        },
        error: function (result) {
            alert("Errore nell'eseguire le modifiche")
        }
    });
}

function editNoteEdit(changeOption) {
    var object = getObjectJson();
    console.log(object)

    $.ajax({
        type: "GET",
        url: "/Transactions/NoteEdit",
        contentType: "application/json;charset=utf-8",
        data: { transaction: JSON.stringify(object) },
        dataType: "json",
        success: function (result) {
            manageToggles(object, changeOption)
            location.reload();
        },
        error: function (result) {
            alert("Errore nell'eseguire le modifiche")
        }
    });
}

function manageToggles(object, changeOption) {
    //location.reload();

    if (changeOption == "check") {
        if (object.isChecked == true) {
            $("#resultCheck").removeClass("label-dangerColor");
            $("#resultCheck").addClass("label-successColor");
            $("#resultCheck").text("Transazione verificata con successo");
        }
        else {
            $("#resultCheck").removeClass("label-successColor");
            $("#resultCheck").addClass("label-dangerColor");
            $("#resultCheck").text("Transazione non verificata");
        }
        $("#resultCheck").show().delay(2000).fadeOut();
    }
    if (changeOption == "archive") {
        if (object.isArchived == true) {
            $("#resultArchive").removeClass("label-dangerColor");
            $("#resultArchive").addClass("label-successColor");
            $("#resultArchive").text("Transazione archiviata con successo");
        }
        else {
            $("#resultArchive").removeClass("label-successColor");
            $("#resultArchive").addClass("label-dangerColor");
            $("#resultArchive").text("Transazione non archiviata");

        }
        $("#resultArchive").show().delay(2000).fadeOut();
    }      
    if (changeOption == "note") {
        location.reload();
       // $("#checkNote").modal('hide');
    }

}

