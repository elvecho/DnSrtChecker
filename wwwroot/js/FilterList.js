$(document).ready(function () {
    $('[data-toggle="tooltip"]').tooltip();
});
function sendFilters(selectedRow) {
    var parameter = createObjectFilters(selectedRow);   
    window.location = "/Transactions/RedirectToTransactionsWithFilters?filtersJson=" + JSON.stringify(parameter);
    //console.log($(selectedRow))

}

function createObjectFilters(selectedRow) {
    var filters = {
        StoreGroup: $(selectedRow).parent()[0].children[0].innerText,
        Store: $(selectedRow).parent()[0].children[1].innerText ,
        ServerRt: $(selectedRow).parent()[0].children[2].innerText,
        Status: $(selectedRow).parent()[0].children[3].children[0].value,
        error: $(selectedRow).parent()[0].children[4].children[0].value,
        NonCompliant: $(selectedRow).parent()[0].children[5].children[0].value,
        NonCompliantOrHasMismatch: $(selectedRow).parent()[0].children[5].children[0].value,
        IsChecked: $(selectedRow).parent()[0].children[5].children[1].value
    }
    return filters;
}



 