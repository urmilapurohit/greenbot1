
$(document).ready(function () {

    $("#excludeReseller").click(function (e) {
        var resellerIds = '';
        var RA = $('.resellerData').val();
        if (RA) {
            resellerIds = RA.join();
        }
        $.ajax({
            url: urlSaveExcludeReseller,
            type: "GET",
            dataType: "json",
            data: { resellerIds: resellerIds, spvManufacturerId: $("#spvManufactureId").val() },
            contentType: 'application/json; charset=utf-8',
            success: function (result) {
                if (result != null && result.status == true) {
                    showSuccessMessage("Reseller excluded from panel successfully");
                }
                else
                    showErrorMessage("Error while excluding reseller from panel");

                $('#popUpReseller').modal("hide");
            },
            error: function (e) {
                console.log(e);
            }
        });
    });

    $("#resetResellerPopup").click(function (e) {
        $('.resellerData')[0].sumo.unSelectAll();
    });

    reseller = $('.resellerData').SumoSelect({ csvDispCount: 3, search: true, searchText: 'Search..', selectAll: true, okCancelInMulti: true });

    window.onload = function () {
        var message = localStorage.getItem('MessageText');
        if (message != null  && message != '' && message != 'null') {
            showSuccessMessage(message);
            localStorage.setItem('MessageText', '');
        }

    }
    var grid = $("#datatableSpvManufacturerList").kendoGrid({

        dataSource: {

            type: "json",
            transport: {
                read: {

                    url: urlGetSpvManufacture,
                    data: {},
                    contentType: "application/json; charset=utf-8",
                    dataType: 'json',
                    type: 'POST'
                },
                parameterMap: function (options) {
                    options.SPVManufactureName = ($("#txtSPVManufactureName").val() == null ? 'All' : $("#txtSPVManufactureName").val());
                    options.ServiceAdministrator = ($("#txtServiceAdministrator").val() == null ? 'All' : $("#txtServiceAdministrator").val());
                    return JSON.stringify(options);
                },
            },
            batch: true,
            schema: {
                data: "data",
                total: "total"
            },
            pageSize: 10,
            serverSorting: true,
            serverPaging: false
        },
        resizable: true,
        pageable: {
            buttonCount: 5,
            pageSizes: [10, 25, 50, 100, 200]
        },
        scrollable: false,
        //reorderable: false,
        dataBound: onDataBound,
        columns: [
            {
                template: "#= ++rowNumber #",
                field: "",
                title: "No.",
                width: "60px"
            },
            {
                field: "SpvUserId",
                title: "SpvUserId",
                width: "100px",
                hidden: true

            },
            {
                field: "SPVManufactureName",
                title: "SPVManufactureName",
                filterable: false
            },
            {
                field: "ServiceAdministrator",
                title: "ServiceAdministrator",
                filterable: false
            },
            {
                field: "ExcludeReseller",
                title: "Exclude Reseller",
                filterable: false,
                template: "#= ExcludeResellerPopup(data)#"
            },
            {
                field: "Supplier",
                title: "Supplier",
                filterable: false,
                template: "#= SupplierManufacturePopUp(data)#"
            },

            {
                field: "IsSpvAllowedBySpvmanufacturer",
                width: "120px",
                template: "#= kendo_IsSpvAllowedBySpvmanufacturer(data)#"
                //template:'<div class="onoffswitch" style="position:relative!important;"><input type = "checkbox" name = "onoffswitchIsSpvAllowedBySpvmanufacturer" class="onoffswitch-checkbox switch" id = "onoffswitchIsSpvAllowedBySpvmanufacturer_' + data.SPVManufactureId + '" ison = "1"> <label class="onoffswitch-label" for="onoffswitchIsSpvAllowedBySpvmanufacturer"><span class="onoffswitch-inner" ></span ><span class="onoffswitch-switch"></span></label ></div >'
            }],


        dataBinding: function () {
            rowNumber = (this.dataSource.page() - 1) * this.dataSource.pageSize();

        },


        //dataBound: function () {
        //    $(".onoffswitchIsSpvAllowedBySpvmanufacturer").each(function (text, value) {
        //        if ($(value).attr("data-check") == "true") {
        //            $(value).prop("checked", true);
        //        }
        //        else
        //            $(value).prop("checked", false);
        //    });
        //    //var view = this.dataSource.view();
        //    //for (var i = 0; i < view.length; i++) {
        //    //    if (checkedIds[view[i].id]) {
        //    //        this.tbody.find("tr[data-uid='" + view[i].uid + "']")
        //    //            .addClass("k-state-selected")
        //    //            .find(".onoffswitchIsSpvAllowedBySpvmanufacturer")
        //    //            .attr("checked", "checked");
        //    //    }
        //    //}

        //}


    }).data("kendoGrid");



    grid.table.on("click", ".onoffswitchIsSpvAllowedBySpvmanufacturer", selectRow);
   

   

    $("#btnSaveManufacturer").bind("click", function () {
        var checked = [];
        checkedIds.forEach(function (e) {
            if (e.value == true) {
                checked.push(e.key);
            }
        });

        //for (var i in checkedIds) {
        //    if (checkedIds[i]) {
        //        checked.push(i);
        //    }
        //}
        var JsonData = {
            Spvmanufacturerid: checked.toString()
        }

        $.ajax({
            url: urlSaveSPVManufacture,
            type: 'post',
            dataType: 'Json',
            data: (JsonData),

            success: function (status) {
                if (status.status == true) {
                    showSuccessMessage("SPV allowance set by particular manufacturers successfully.");
                }
                else {

                    showErrorMessage("SPV allowance could not set by particular manufacturers successfully.");
                }
            },

            error: function () {
                showErrorMessage("SPV allowance could not set by particular manufacturers successfully.");
            }
        });


    });

    $("#onoffswitchAllowSPV").change(function (ev) {
        var checked = ev.target.checked;
        $(".onoffswitchIsSpvAllowedBySpvmanufacturer").each(function (text, value) {
            $(value).prop("checked", checked);
            for (var i = 0; i < checkedIds.length; i++) {
                checkedIds[i].value = checked;
                    
                }
            });
       
    });

});
function kendo_IsSpvAllowedBySpvmanufacturer(data) {
    var DynamicSwitch = '<div class="onoffswitch" style="position:relative!important;"><input type="checkbox" data-check="' + data.IsSpvAllowedBySpvmanufacturer + '" name="onoffswitchIsSpvAllowedBySpvmanufacturer" class="onoffswitch-checkbox switch onoffswitchIsSpvAllowedBySpvmanufacturer" id="' + data.SPVManufactureId + '" ison="1"> <label class="onoffswitch-label" for="' + data.SPVManufactureId + '"><span class="onoffswitch-inner" ></span ><span class="onoffswitch-switch"></span></label ></div >';
    return DynamicSwitch;
}

function SetSPVByManufacturer() {
    $("#datatableSpvManufacturerList").kendoGrid({
        dataSource: {
            type: "odata",
            transport: {
                read: {
                    url: urlGetSpvManufacture,
                    data: {},
                    contentType: 'application/json',
                    dataType: 'json',
                    type: 'POST'
                },
                parameterMap: function (options) {
                    options.SPVManufactureName = ($("#txtSPVManufactureName").val() == null ? 'All' : $("#txtSPVManufactureName").val());
                    options.ServiceAdministrator = ($("#txtServiceAdministrator").val() == null ? 'All' : $("#txtServiceAdministrator").val());
                    return JSON.stringify(options);
                },

            },

            schema: {
                data: "data",
                total: "total"
            },
            pageSize: "10",
            serverPaging: true,
            serverSorting: true,
            requestStart: function (e) {
                setTimeout(function (e) {
                    $(".k-loading-image").hide();
                });
            }
        },

        pageable: {
            buttonCount: 5,
            pageSizes: [10, 25, 50, 100, 200]
        },
        groupable: false,
        sortable: true,
        resizable: false,
        scrollable: false,
        reorderable: false,
        filterMenuInit: function (e) {
            $(".loading-image").hide();
        },

        columns: [
            {
                template: "#= ++rowNumber #",
                field: "",
                title: "No.",
                width: "60px"
            },
            {
                field: "SpvUserId",
                title: "SpvUserId",
                width: "100px",
                hidden: true,
                template: function (data) {
                    return "<span>" + data.SerialNumber + "</span>";
                },
            },
            {
                field: "SPVManufactureName",
                title: "SPVManufactureName",
                filterable: false,
                sortable: true,
                template: function (data) {
                    return "<span>" + data.SPVManufactureName + "</span>";
                },
                media: "(min-width: 100px)"
            },
            {
                field: "ServiceAdministrator",
                title: "ServiceAdministrator",
                filterable: false,
                sortable: true,
                template: function (data) {
                    return "<span>" + data.ServiceAdministrator + "</span>";
                },
            },

            {
                field: "ExcludeReseller",
                title: "Exclude Reseller",
                filterable: false,
                template: "#= ExcludeResellerPopup(data)#"
            },

            {
                field: "Supplier",
                title: "Supplier",
                filterable: false,
                sortable: false,
                template: "#= SupplierManufacturePopUp(data)#"
            },

            {
                field: "IsSpvAllowedBySpvmanufacturer",
                width: "120px",
                sortable: false,
                template: "#= kendo_IsSpvAllowedBySpvmanufacturer(data)#"

            }],
        dataBinding: function () {
            rowNumber = (this.dataSource.page() - 1) * this.dataSource.pageSize();

        },
        dataBound: function () {
            $(".onoffswitchIsSpvAllowedBySpvmanufacturer").each(function (text, value) {
                if ($(value).attr("data-check") == "true") {
                    $(value).prop("checked", true);
                }
                else
                    $(value).prop("checked", false);
            });

        }

    });

}
function initeSPV() {

    var grid = $("#datatableSpvManufacturerList").data("kendoGrid");

    var data = grid.dataSource.data();
    var totalNo = data.length;
}

var checkedIds = [];
function selectRow() {
    var checked = this.checked,
        row = $(this).closest("tr"),
        grid = $("#datatableSpvManufacturerList").data("kendoGrid"),
        dataItem = grid.dataItem(row);

    var id = dataItem.SPVManufactureId;
    checkedIds.forEach(function (e) {
        if (e.key == id) {
            e.value = checked;
            
            return false;
        }
    });
    var switchValue;
    checkedIds.forEach(function (e) {
        if (e.value == false) {
            switchValue = false;
        }
        
    });
    if (switchValue==false)
        $("#onoffswitchAllowSPV").prop("checked", false);
    else
        $("#onoffswitchAllowSPV").prop("checked", true);
    //checkedIds[id].value = checked;
    //checkedIds.push({ id : checked });

    //checkedIds[dataItem.SPVManufactureId] = checked;

}

function onDataBound(e) {

    var view = this.dataSource.view();
    for (var i = 0; i < view.length; i++) {
        if (checkedIds[view[i].SPVManufactureId]) {
            this.tbody.find("tr[data-uid='" + view[i].uid + "']")
                .find(".onoffswitchIsSpvAllowedBySpvmanufacturer")
                .attr("checked", "checked");
        }
    }
    var grid = $("#datatableSpvManufacturerList").data("kendoGrid");
    var dataSource = grid.dataSource.data();
    var totalNo = dataSource.length;
    var manufactureId = '';
    if (checkedIds.length == 0) {
        for (var i = 0; i < totalNo; i++) {
            manufactureId = dataSource[i].SPVManufactureId;
            if ($("#datatableSpvManufacturerList").data("kendoGrid").dataSource.data()[i].IsSpvAllowedBySpvmanufacturer == true) {
                checkedIds.push({ key: manufactureId, value: true });
            }
            else {
                checkedIds.push({ key: manufactureId, value: false });
            }
            //});
        }

    }

   
        $(".onoffswitchIsSpvAllowedBySpvmanufacturer").each(function (text, value) {

            checkedIds.forEach(function (e) {
                if (e.key == value.id) {
                    $(value).prop("checked", e.value);
                    return false;
                }
            });
            
        //if(value.id == )
   
   
    });

    var switchValue;
    checkedIds.forEach(function (e) {
        if (e.value == false) {
            switchValue = false;
        }

    });
    if (switchValue == false)
        $("#onoffswitchAllowSPV").prop("checked", false);
    else
        $("#onoffswitchAllowSPV").prop("checked", true);

    //checkedIds.forEach(function (e) {
    //    if (e.value == false) {
    //        $("#onoffswitchAllowSPV").prop("checked", false);
    //        return false;
    //    }
       
    //});
}

function ResetSearchFilters() {
    $("#txtSPVManufactureName").val('');
    $("#txtServiceAdministrator").val('');
    Searching('', '');
    $("#txtSPVManufactureName").focus();
}
function SupplierManufacturePopUp(data) {
    
    var supplierText = "<a href='#' onclick=GetSupplier(" + data.SPVManufactureId + ")>View Supplier</a>";
    return supplierText;
}

function ExcludeResellerPopup(data) {
    var resellerText = "<a href='#' onclick=GetReseller(" + data.SPVManufactureId + ")>View Reseller</a>";
    return resellerText;
}

function GetExcludedReseller(SPVManufactureID) {
    $.ajax({
        url: urlGetExcludedReseller,
        type: "GET",
        dataType: "json",
        data: { spvManufacturerId: SPVManufactureID },
        async: false,
        contentType: 'application/json; charset=utf-8',
        success: function (result) {
            if (result != null && result.data && result.data.length > 0) {
                var ids = result.data.split(',');
                for (var i = 0; i < ids.length; i++) {
                    $(".resellerData")[0].sumo.selectItem(ids[i].toString());
                }
            }
            $('#popUpReseller').find("input[id='spvManufactureId']").val(SPVManufactureID);
            $('#popUpReseller').modal("show");
        },
        error: function (e) {
            console.log(e);
        }
    });
}

function GetReseller(SPVManufactureID) {
    var num = $("#ResellerDiv").find('ul').find('li').length;
    if (num == 0) {
        $.ajax({
            url: urlGetReseller,
            type: "GET",
            dataType: "json",
            data: '',
            contentType: 'application/json; charset=utf-8',
            success: function (result) {
                //$("#ResellerDiv").find('ul').find('li').remove();
                if (result != null && result.length > 0) {
                    for (var i = 0; i < result.length; i++) {
                        $('.resellerData')[0].sumo.add(result[i].Value, result[i].Text);
                    }
                    GetExcludedReseller(SPVManufactureID);
                }
            },
            error: function (e) {
                console.log(e);
            }
        });
    }
    else {
        $('.resellerData')[0].sumo.unSelectAll();
        GetExcludedReseller(SPVManufactureID);
        $('#popUpReseller').find("input[id='spvManufactureId']").val(SPVManufactureID);
        $('#popUpReseller').modal("show");
    }
}

function GetSupplier(SPVManufactureID) {
    $.ajax({
        url: urlGetSupplier,
        type: 'GET',
        data: { id: SPVManufactureID },
        contentType: 'application/json',
        dataType: 'json',
        success: function (data) {
            if (data.lstSupplier.length > 0) {

                $('#supplierUl').html('');
                for (var i = 0; i < data.lstSupplier.length; i++) {
                    $("#supplierUl").append('<li>' + data.lstSupplier[i] + '</li>');
                }
            }
        }
    });
    $('#popUpSupplier').modal("show");
}

$("#txtSPVManufactureName").keypress(function (e) {
    if (e.which == 13) {
        $('#btnSendRequest').click();
    }

});

$("#btnReset").click(function (e) {
    $("#txtSPVManufactureName").val('');
    $("#txtServiceAdministrator").val('');
    SearchSPVManufactureName();
});

$('#btnSendRequest').click(function (e) {
    SearchSPVManufactureName();
});

function kendo_IsSpvAllowedBySpvmanufacturer(data) {
    var DynamicSwitch = '<div class="onoffswitch" style="position:relative!important;"><input type="checkbox" data-check="' + data.IsSpvAllowedBySpvmanufacturer + '" name="onoffswitchIsSpvAllowedBySpvmanufacturer" class="onoffswitch-checkbox switch onoffswitchIsSpvAllowedBySpvmanufacturer" id="' + data.SPVManufactureId + '" ison="1"> <label class="onoffswitch-label" for="' + data.SPVManufactureId + '"><span class="onoffswitch-inner" ></span ><span class="onoffswitch-switch"></span></label ></div >';
    return DynamicSwitch;
}
function Searching(name, serviceadmin) {
    var serviceadmin = serviceadmin;
    var name = name;
    SetSPVByManufacturer(name, serviceadmin);
}

function SearchSPVManufactureName() {
    var name = $("#txtSPVManufactureName").val();
    var serviceadmin = $("#txtServiceAdministrator").val();
    SetSPVByManufacturer(name, serviceadmin);
}

function saveSpvSetByManufacturerPopUp() {
    var chkArray = [];
    $(".onoffswitchIsSpvAllowedBySpvmanufacturer:checked").each(function () {
        chkArray.push($(this).attr("id"));
    });
    var JsonData = {
        Spvmanufacturerid: chkArray.toString()
    }
    $.ajax({
        url: urlSaveSPVManufacture,
        type: 'post',
        dataType: 'Json',
        data: (JsonData),
        success: function (status) {
            if (status.status == true) {
                showSuccessMessage("SPV allowance set by particular manufacturers successfully.");
            }
            else {
                showErrorMessage("SPV allowance could not set by particular manufacturers successfully.");
            }
        },
        error: function () {
            showErrorMessage("SPV allowance could not set by particular manufacturers successfully.");
        }
    });
}

function showErrorMessage(message) {
    $(".alert").hide();
    $("#successMsgRegion").hide();
    $("#errorMsgRegion").html(closeButton + message);
    $("#errorMsgRegion").show();
}

function showSuccessMessage(message) {
    $(".alert").hide();
    $("#errorMsgRegion").hide();
    $("#successMsgRegion").html(closeButton + message);
    $("#successMsgRegion").show();
}

$("#btnUploadSPVJson").change(function () {
    var fileUpload = $("#btnUploadSPVJson").get(0);
    var files = fileUpload.files;
    var fileData = new FormData();
    if (files[0].name.split('.').pop().toLowerCase() == 'json') {
        fileData.append(files[0].name, files[0]);
    }
    else {
        alert('Please upload json format file');
        return false;
    }
    $.ajax({
        url: urlUploadJson,
        type: "POST",
        data: fileData,
        contentType: false, // Not to set any content header
        processData: false, // Not to process data                   
        success: function (result) {
            if (result.status) {
                showSuccessMessage("SPV Json file upload successfully.");
                $("#headerTitle").html(result.message[0]);
                localStorage.setItem("MessageText", "SPV Json file upload successfully.");
                refreshKendoGrid();
            
            }
            else {
                showErrorMessage("something wrong was happening in upload SPV Json.");
            }
        },
    });
});
$("#btnSyncSPVJson").click(function () {
    $.ajax({
        url: urlSyncJson,
        type: "GET",
        contentType: false, // Not to set any content header
        processData: false, // Not to process data
        data: {},
        success: function (result) {
            if (result.status) {
                showSuccessMessage("SPV Json Sync successfully.");
                $("#headerTitle").html(result.message[0]);
                localStorage.setItem("MessageText", "SPV Json Sync successfully.");
                refreshKendoGrid();
                
            }
            else {
                showErrorMessage("something wrong was happening in sync SPV Json.");
            }
        },
    });
})
function refreshKendoGrid() {
   location.reload(true);
   
    //checkedIds = [];
    //var grid = $("#datatableSpvManufacturerList").data("kendoGrid");
    //grid.refresh();
}