function AssignSCO() {

    var selectedRows = [];
    $('#datatable tbody input:checkbox').each(function () {
        if ($(this).prop('checked') == true) {
            selectedRows.push($(this).attr('jobid'));
        }
    });

    if (selectedRows.length == 0) {
        alert("Please select job(s)");
        return false;
    }

    if ($("#hdnsolarCompanyid").val() == -1) {
        alert("Please select one solarcompany");
        return false;
    }

    $("#loading-image").show();

    scaId = (scaId == 0) ? $("#hdnsolarCompanyid").val() : scaId ;

    setTimeout(function () {
            $.get(urlAssignSCO, { jobIds: selectedRows.toString(), solarcomapnyId: scaId },function (data) {
            $('#divAssignSco').empty();
            $('#divAssignSco').append(data);
            $('#SCOpopup').modal({ backdrop: 'static', keyboard: false });
            $('#ScoID').focus();
            $("#loading-image").hide();
        });
    }, 500);

}