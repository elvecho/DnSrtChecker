
$(document).ready(function () {
    
    if ($(".clickableHome").length == 1) {
        $(".bodyCollapseHome").removeAttr("style");
    } else {
        $(".bodyCollapseHome:first").css("display", "");

    }

    $("#transmissionDateFrom").datepicker({
        setDate: new Date(),
        pickTime: false,
        dateFormat: "dd-mm-yy",
        autoclose: true

    });

    $("#transmissionDateTo").datepicker({
        setDate: new Date(),
        pickTime: false,
        autoclose:true,
        dateFormat: "dd-mm-yy"
    });
    
    $('.calendar-from').click(function () {
        $("#transmissionDateFrom").focus();
    });

    $('.calendar-to').click(function () {
        $("#transmissionDateTo").focus();
    });



    $("#transmissionDateFrom").datepicker({
        setDate: new Date(),
        pickTime: false,
        dateFormat: "dd-mm-yy",
        autoclose: true,
        onSelect: function (dateText, inst) {
            if ($('#transmissionDateFrom').datepicker().val() != "") {
                //var date2 = $('#transmissionDateFrom').datepicker('getDate');
                //var date2 = $(this).datepicker('getDate');
                var date2 = $(this).val();

                console.log(date2)
                var minDate = new Date();
                // set min date
                minDate.setDate(new Date(date2).getDate());
                minDate.setMonth(new Date(date2).getMonth());
                minDate.setFullYear(new Date(date2).getFullYear());
                //sets minDate to from transmissionDateFrom
                $('#transmissionDateTo').datepicker('option', 'minDate', minDate);
                $('#transmissionDateTo').datepicker('option', 'defaultDate', minDate);
                $('#transmissionDateTo').focus();
            }
            else {
                $('#transmissionDateTo').datepicker('option', 'minDate', null);

            }
        }

    }).on('change select', function () {
        if ($('#transmissionDateFrom').datepicker().val() != "") {
            var date2 = $('#transmissionDateFrom').datepicker('getDate');
            //var date2 = $('#transmissionDateFrom').val();
            var minDate = new Date();
            // set min date
            minDate.setDate(new Date(date2).getDate());
            minDate.setMonth(new Date(date2).getMonth());
            minDate.setFullYear(new Date(date2).getFullYear());
            //sets minDate to from transmissionDateFrom
            $('#transmissionDateTo').datepicker('option', 'minDate', minDate);
            $('#transmissionDateTo').datepicker('option', 'defaultDate', minDate);
            $('#transmissionDateTo').focus();
        }
        else {
            $('#transmissionDateTo').datepicker('option', 'minDate', null);

        }
    })
        ;

    (function blink() {
       
        $('.blink_error').fadeOut(500).fadeIn(500, blink);
    })();

});


//Collapsable rows table
// Old onclick before 03 september 2020
//$('.clickableHome').click(function () {
//    var id = $(this).attr('id');
//    console.log(id);
//    var listRes = getTrxDayHome(id);
//    var result =  listRes;
//    $(".bodyCollapseHome").not(this).hide();
//    $(this).parent().next('.bodyCollapseHome').toggle();
//    console.log($(this).parents());
//    $(this).parents()[0].scrollIntoView();

//});
// 03 september 2020 edit onclick
$('.clickableHome').click(function () {

    var id = $(this).attr('id');
    console.log(id);
    var selectedItem = $(this);

    $(this).children(".spinnerHome")[0].classList.remove("hidden");

    getTrxDayHome2(id, function (handleData) {

        console.log(handleData);
        selectedItem.parent().siblings(".panel-body").children("table").children("tbody").html(handleData);
        $(".bodyCollapseHome").not(selectedItem).hide();
        selectedItem.parent().next('.bodyCollapseHome').toggle();
        console.log(selectedItem.parents());
        selectedItem.parents()[0].scrollIntoView();
        selectedItem.children(".spinnerHome")[0].classList.add("hidden");

    });
    

});
$("#btnFilter").click(function () {
    var dateFrom = $('#transmissionDateFrom').datepicker().val();
    var dateTo = $('#transmissionDateTo').datepicker().val();
    
    if (isEmpty(dateFrom) && !isEmpty(dateTo)) {
        
        $('#transmissionDateFrom').attr("oninvalid", "this.setCustomValidity('Sceglie prima una data d'inizio')");
        $('#transmissionDateFrom').prop('required', true);

    }
    if (!isEmpty(dateFrom) && isEmpty(dateTo)) {
        
        $('#transmissionDateTo').attr("oninvalid", "this.setCustomValidity('Sceglie prima una data di fine')");
        $('#transmissionDateTo').prop('required', true);


    }

    //if (isEmpty(dateFrom) && isEmpty(dateTo)) {
    //    dateTo = new Date().toDateString();
    //    var d = new Date(dateTo);
    //    d.setDate(d.getDate() - 30);
    //    dateFrom= d;
    //    console.log("datefFrom   " + dateFrom);
    //    console.log("dateTo    " + dateTo);
    //}

    $('#transmissionDateFrom').removeProp('required');
    $('#transmissionDateFrom').removeAttr("oninvalid");
    $('#transmissionDateTo').removeProp('required');
    $('#transmissionDateTo').removeAttr("oninvalid");

});



function getTrxDayHome(date) {
    var srvObject = {
        storeGroup: $("#storeGroup").val(),
        store: $("#store").val(),
        serverRt: $("#serverRt").val(),
        status: $("#status").val(),
        nonCompliant: $("#nonCompliant").val(),
        error: $("#error").val(),
        transmissionDateFrom: $("#transmissionDateFrom").val(),
        transmissionDateTo: $("#transmissionDateTo").val(),
        conformity: $("#nonCompliant").val(),
        dayFilter: date,
    };

    var res = "";
    $.ajax({
        type: "GET",
        url: "/Home/GetListOfDay",
        contentType: "application/json;charset=utf-8",
        async: false,
        data: srvObject,// JSON.parse(srvObject),//{ filters: srvObject},
        dataType: "html",
        success: function (result) {

            if (result.success != "" || result.success != null) {
                console.log(result)
                $("#" + date).parent().siblings(".panel-body").children("table").children("tbody").html(result)
            }
            else {
                alert("Errore nell'eseguire le modifiche")

            }
        }
    });
    return res;
}


function getTrxDayHome2(date,handleData) {
    var srvObject = {
        storeGroup: $("#storeGroup").val(),
        store: $("#store").val(),
        serverRt: $("#serverRt").val(),
        status: $("#status").val(),
        nonCompliant: $("#nonCompliant").val(),
        error: $("#error").val(),
        transmissionDateFrom: $("#transmissionDateFrom").val(),
        transmissionDateTo: $("#transmissionDateTo").val(),
        conformity: $("#nonCompliant").val(),
        dayFilter: date,
    };

    var res = "";
    $.ajax({
        type: "GET",
        url: "/Home/GetListOfDay",
        contentType: "application/json;charset=utf-8",
       // async: false,
        data: srvObject,// JSON.parse(srvObject),//{ filters: srvObject},
        dataType: "html",
        success: function (result) {
            if (result.success != "" || result.success != null) {
                handleData(result);
                res = result;
                console.log(result);
               // $("#" + date).parent().siblings(".panel-body").children("table").children("tbody").html(result)
            }
            else {
                alert("Errore nell'eseguire le modifiche")

            }
        }
    });
    //return res;
}