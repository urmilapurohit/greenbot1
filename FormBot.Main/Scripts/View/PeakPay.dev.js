$(document).ready(function () {
    $("#ResellerId").select2();
    $('#searchSolarCompany').autocomplete({
        minLength: 0,
        source: function (request, response) {
            var data = [];
            $("#hdnsolarCompanyid").val(0);
            $.each(solarCompanyList, function (key, value) {
                if (value.text.toLowerCase().indexOf($("#searchSolarCompany").val().toLowerCase()) > -1) {
                    data.push({ Title: value.text, id: value.value });
                }
            });

            response($.map(data, function (item) {
                return { label: item.Title, value: item.Title, id: item.id };
            }))
        },
        select: function (event, ui) {
            $("#hdnsolarCompanyid").val(ui.item.id); // save selected id to hidden input
        }
    }).on('focus', function () { $(this).keydown(); });

    $.ui.autocomplete.prototype._renderItem = function (ul, item) {
        var re = new RegExp($.trim(this.term.toLowerCase()));
        var t = item.label.replace(re, "<span style='font-weight:600;'>" + $.trim(this.term.toLowerCase()) +
            "</span>");
        return $("<li style='font-size:14px;'></li>")
            .data("item.autocomplete", item)
            .append("<a>" + t + "</a>")
            .appendTo(ul);
    };


    if (UserTypeID == 1 || UserTypeID == 3) {
        var IsResellerExist = false;
        $.ajax({
            type: 'get',
            url: url_GetReseller,
            dataType: 'json',
            data: '',
            async: false,
            success: function (reseller) {
                $.each(reseller, function (i, res) {
                    $("#ResellerId").append('<option value="' + res.Value + '">' + res.Text + '</option>');
                    if (IsResellerExist == false && res.Value == localStorage.getItem('PeakPay_Reseller')) {
                        IsResellerExist = true;
                    }
                });

                var resellerID = PeakPay_Reseller;

                if (resellerID == '' || resellerID == '0') {
                    if (IsResellerExist) {
                        document.getElementById("ResellerId").value = localStorage.getItem('PeakPay_Reseller')
                    }
                    else {
                        $("#ResellerId").val($("#ResellerId option:first").val());
                        localStorage.setItem('PeakPay_Reseller', $("#ResellerId option:first").val());
                    }
                }
                else {

                    $("#ResellerId").val(resellerID);
                }
                BindSolarCompany(localStorage.getItem('PeakPay_Reseller'), 0);
            },
            error: function (ex) {
                alert('Failed to retrieve Resellers.' + ex);
            }
        });
    }
    else if (UserTypeID == 2) {
        BindSolarCompany(localStorage.getItem('PeakPay_Reseller'), 0);
    }
    else if (UserTypeID == 5) {
        if (isAllScaJobView == "true")
            BindSolarCompany(ResellerId, 0)
        else
            BindSolarCompany(0, ProjectSession_LoggedInUserId);
    }

    FromToDate($('#txtCERApprovedFromDate'), $('#txtCERApprovedToDate'));
    FromToDate($('#txtSettleBeforeFromDate'), $('#txtSettleBeforeToDate'));
    FromToDate($('#txtPaymentFromDate'), $('#txtPaymentToDate'));

    $('#datatable').DataTable({
        destroy: true,
        iDisplayLength: 10,
        lengthMenu: PageSize,
        language: {
            sLengthMenu: "Page Size: _MENU_"
        },
        columns: [
                {
                    'data': 'Id',
                    "orderable": false,
                    "render": function (data, type, full, meta) {
                        if ($('#chkAll').prop('checked') == true) {
                            return '<input type="checkbox"  stcStatusId="' + full.STCStatusId + '" jobId="' + full.JobID + '" stcJobDetailsId="' + full.STCJobDetailsID + '" stcPvdCode="' + full.STCPVDCode + '" isInvoiced="' + full.IsInvoiced + '" settlementTerm="' + full.STCSettlementTerm + '" stcPrice="' + full.STCPrice + '" STCInvoiceCnt="' + full.STCInvoiceCnt + '" checked >';
                        }
                        else {
                            return '<input type="checkbox"  stcStatusId="' + full.STCStatusId + '" jobId="' + full.JobID + '" stcJobDetailsId="' + full.STCJobDetailsID + '" stcPvdCode="' + full.STCPVDCode + '" isInvoiced="' + full.IsInvoiced + '" settlementTerm="' + full.STCSettlementTerm + '" stcPrice="' + full.STCPrice + '" STCInvoiceCnt="' + full.STCInvoiceCnt + '">';
                        }
                    }
                },

                {
                    'data': 'Id',
                    "render": function (data, type, full, meta) {
                        return full.JobID;
                    },
                    "orderable": false
                },

                {
                    'data': 'Reference',
                    "render": function (data, type, full, meta) {
                        if (full.Reference != null) {
                            var url = urlJobIndex + "?id=" + full.Id;
                            return '<a href="' + url + '" style="text-decoration:none;" target="_blank">' + full.Reference + '</a>'
                        }
                        else {
                            return '';
                        }
                    },
                },

                 {
                     'data': 'OwnerName',
                     "render": function (data, type, full, meta) {
                         if (full.OwnerName != null) {
                             return full.OwnerName
                         }
                         else {
                             return '';
                         }
                     },
                 },

                {
                    'data': 'InstallationAddress',
                    "render": function (data, type, full, meta) {
                        return full.InstallationAddress
                    }
                },

                {
                    'data': 'StcStatus',
                    "render": function (data, type, full, meta) {
                        return full.StcStatus
                    }
                },

                {
                    'data': 'SolarCompany',
                    "render": function (data, type, full, meta) {
                        $("#lblTotalAmount").html(meta.settings.json.iTotalAmount);
                        return full.SolarCompany
                    }
                },

                {
                    'data': 'SubmissionDate',
                    "render": function (data, type, full, meta) {
                        return ConvertToDateWithFormat(data, dateFormat);
                    }
                },

                {
                    'data': 'CERApprovedDate',
                    "render": function (data, type, full, meta) {
                        return ConvertToDateWithFormat(data, dateFormat);
                    }
                },

                {
                    'data': 'SettleBefore',
                    "render": function (data, type, full, meta) {
                        return ConvertToDateWithFormat(data, dateFormat);
                    }
                },

                {
                    'data': 'DaysLeft',
                    "render": function (data, type, full, meta) {
                        return full.DaysLeft;
                    }
                },

                {
                    'data': 'PaymentDate',
                    "render": function (data, type, full, meta) {
                        return ConvertToDateWithFormat(data, dateFormat);
                    }
                },

                {
                    'data': 'STCAmount', "sClass": "dt-right",
                    "render": function (data, type, full, meta) {
                        //return full.STCAmount;
                        return '<lable class="clsLabel">' + PrintDecimal(full.STCAmount) + '</lable>';
                    }
                },

                {
                    'data': 'STCPrice',
                    "render": function (data, type, full, meta) {
                        return PrintDecimal(full.STCPrice);
                    }
                },

                {
                    'data': 'IsGst',
                    "render": function (data, type, full, meta) {
                        if (full.IsGst) {
                            return "Yes";//'<img src="../Images/ar_green.png"/>';
                        }
                        else {
                            return "No";//'<img src="../Images/ic_reject.png"/>';
                        }
                    },
                    "orderable": false
                },

                {
                    'data': 'STCFee',
                    "render": function (data, type, full, meta) {
                        return PrintDecimal(full.STCFee);
                    }
                },

                {
                    'data': 'Total',
                    "render": function (data, type, full, meta) {
                        //return full.Total;
                        return '<lable class="lblTotal">' + PrintDecimal(full.Total) + '</lable>';
                    },
                    "orderable": false
                },

                {
                    'data': 'IsInvoiced',
                    "render": function (data, type, full, meta) {
                        if (full.IsInvoiced == 1 || full.STCInvoiceCnt > 0) {
                            return '<img src="../images/ar_green.png"/><span style="display:none">sent</span>';
                        }
                        else if (full.IsInvoiced == 2) {
                            return '<img src="../images/ic_reject.png"/><span style="display:none">unsent</span>';
                        }
                        else if (full.IsInvoiced == 3) {
                            return '<img src="../images/ic_readytotrade.png"/><span style="display:none">ready to sent</span>';
                        }
                        else
                            return '';
                    },
                    "orderable": false
                },
        ],

        dom: "<<'table-responsive tbfix't><'paging grid-bottom prevar'p><'bottom'l><'clear'>>",
        initComplete: function (settings, json) {
            //$("#lblTotalAmount").html(json.iTotalAmount);
            $('.grid-bottom span:first').attr('id', 'spanMain');
            $("#spanMain span").html("");
        },
        "fnRowCallback": function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {
        },

        bServerSide: true,
        sAjaxSource: urlGetPeakPayList,

        "fnDrawCallback": function (settings, json) {
            $("#btotalTradedSTC").html(0);

            $("#lblTotalAmountSelected").html(parseFloat(0).toFixed(2));
            $("#lblTotalAmountViewable").html(parseFloat(0).toFixed(2));

            var tViewablwAmount = 0;
            $('#datatable tbody tr').each(function () {
                var lblTotal = $(this).find('.lblTotal').text();
                var cTotal = (lblTotal == '' || lblTotal == undefined) ? 0 : lblTotal;
                tViewablwAmount = parseFloat(parseFloat(tViewablwAmount) + parseFloat(cTotal)).toFixed(2);
            })
            $("#lblTotalAmountViewable").html(tViewablwAmount);

            if ($('#chkAll').prop('checked') == true) {
                chkCount = $('#datatable >tbody >tr').length;
                var tSelsectedAmount = 0;
                var tSTC = 0;
                $('#datatable tbody input[type="checkbox"]').each(function () {
                    var lblTotal = $(this).parent().parent().find('.lblTotal').text();
                    var cTotal = (lblTotal == '' || lblTotal == undefined) ? 0 : lblTotal;
                    tSelsectedAmount = parseFloat((parseFloat(tSelsectedAmount) + parseFloat(cTotal))).toFixed(2);

                    var cSTC = ($(this).parent().parent().find('.clsLabel').text() == '' || $(this).parent().parent().find('.clsLabel').text() == undefined) ? 0 : $(this).parent().parent().find('.clsLabel').text();
                    tSTC = (parseFloat(parseFloat(tSTC) + parseFloat(cSTC)).toFixed(2));
                })
                $("#lblTotalAmountSelected").html(tSelsectedAmount);
                $("#btotalTradedSTC").html(tSTC);
            }
            else {
                chkCount = 0;
                $("#lblTotalAmountSelected").html(parseFloat(0).toFixed(2));
                $("#btotalTradedSTC").html(0);
            }

            $("#datatable_wrapper").find(".bottom").find(".dataTables_paginate").remove();
            $(".paging a.previous").html("&nbsp");
            $(".paging a.next").html("&nbsp");
            $('.grid-bottom span:first').attr('id', 'spanMain');
            $("#spanMain span").html("");
            $(".ellipsis").html("...");

            if ($(".paging").find("span").length >= 1) {
                $("#datatable_length").show();
            } else if ($("#datatable").find("tr").length > 11) { $("#datatable_length").show(); } else { $("#datatable_length").hide(); }

            //--------To show display records from total records-------------------
            var table = $('#datatable').DataTable();
            var info = table.page.info();
            if (info.recordsTotal == 0) {
                document.getElementById("numRecords").innerHTML = '<b>' + 0 + '</b>  of  <b>' + info.recordsTotal + '</b> STC Job(s) found';
                $("#lblTotalAmount").html(parseFloat(0).toFixed(2));
            }
            else {
                document.getElementById("numRecords").innerHTML = '<b>' + $('#datatable >tbody >tr').length + '</b>  of  <b>' + info.recordsTotal + '</b> STC Job(s) found';
            }
            GetPeakPayJobStageCount();
            //------------------------------------------------------------------------------------------------------------------------------
        },

        "fnServerParams": function (aoData) {           
            aoData.push({ "name": "stageid", "value": SelectedStageId });
            if (UserTypeID == 1 || UserTypeID == 3) {
                aoData.push({ "name": "reseller", "value": document.getElementById("ResellerId").value });
                //aoData.push({ "name": "reseller", "value": localStorage.getItem('PeakPay_Reseller') });
            }
            if (UserTypeID == 1 || UserTypeID == 3 || UserTypeID == 2 || UserTypeID == 5) {
                aoData.push({ "name": "solarcompanyId", "value": $('#hdnsolarCompanyid').val() });
                //aoData.push({ "name": "solarcompanyId", "value": localStorage.getItem('PeakPay_SolarCompanyId') });
            }
            aoData.push({ "name": "searchText", "value": $("#txtSearchfor").val() });
            aoData.push({ "name": "stcFromPrice", "value": $("#txtStcFromPrice").val() });
            aoData.push({ "name": "stcToPrice", "value": $("#txtStcToPrice").val() });
            aoData.push({ "name": "cerApprovedFromDate", "value": $("#txtCERApprovedFromDate").val() });
            aoData.push({ "name": "cerApprovedToDate", "value": $("#txtCERApprovedToDate").val() });
            aoData.push({ "name": "settleBeforeFromDate", "value": $("#txtSettleBeforeFromDate").val() });
            aoData.push({ "name": "settleBeforeToDate", "value": $("#txtSettleBeforeToDate").val() });
            aoData.push({ "name": "paymentFromDate", "value": $("#txtPaymentFromDate").val() });
            aoData.push({ "name": "paymentToDate", "value": $("#txtPaymentToDate").val() });
            //aoData.push({ "name": "SettlementTermId", "value": $("#SettlementTermId").val() });
            
            aoData.push({ "name": "isSentInvoice", "value": document.getElementById('chkSentInvoice').checked });
            aoData.push({ "name": "isUnsentInvoice", "value": document.getElementById('chkUnsentInvoice').checked });
            aoData.push({ "name": "isReadytoSentInvoice", "value": document.getElementById('chkReadytoSTCInvoice').checked });
            aoData.push({ "name": "systSize", "value": $('#SystemSize').val() });
            aoData.push({ "name": "isAllScaJobView", "value": isAllScaJobView })
            aoData.push({ "name": "defaultGrid", "value": defaultGrid })
        }
    });

    var table = $('#datatable').DataTable();

    $('#chkAll').on('click', function () {
        var rows = table.rows({ 'search': 'applied' }).nodes();
        $('input[type="checkbox"]', rows).prop('checked', this.checked);
        chkCount = this.checked ? $('#datatable >tbody >tr').length : 0;

        if (this.checked) {
            var tSelsectedAmount = 0;
            var tSTC = 0;
            $('#datatable tbody input[type="checkbox"]').each(function () {
                var cTotal = ($(this).parent().parent().find('.lblTotal').text() == '' || $(this).parent().parent().find('.lblTotal').text() == undefined) ? 0 : $(this).parent().parent().find('.lblTotal').text();
                tSelsectedAmount = (parseFloat(tSelsectedAmount) + parseFloat(cTotal));

                var cSTC = ($(this).parent().parent().find('.clsLabel').text() == '' || $(this).parent().parent().find('.clsLabel').text() == undefined) ? 0 : $(this).parent().parent().find('.clsLabel').text();
                tSTC = (parseFloat(parseFloat(tSTC) + parseFloat(cSTC)).toFixed(2));
            })
            $("#lblTotalAmountSelected").html(parseFloat(tSelsectedAmount).toFixed(2));
            $("#btotalTradedSTC").html(tSTC);
        }
        else {
            $("#lblTotalAmountSelected").html(parseFloat(0).toFixed(2));
            $("#btotalTradedSTC").html(0);
        }

    });

    $('#datatable tbody').on('change', 'input[type="checkbox"]', function () {
        if (this.checked) {
            chkCount++;
            if (chkCount == $('#datatable >tbody >tr').length) {
                $('#chkAll').prop('checked', this.checked)
            }
            var cTotal = ($(this).parent().parent().find('.lblTotal').text() == '' || $(this).parent().parent().find('.lblTotal').text() == undefined) ? 0 : $(this).parent().parent().find('.lblTotal').text();
            var tSelsectedAmount = $("#lblTotalAmountSelected").html();
            $("#lblTotalAmountSelected").html(parseFloat(parseFloat(cTotal) + parseFloat(tSelsectedAmount)).toFixed(2));


            var cSTC = ($(this).parent().parent().find('.clsLabel').text() == '' || $(this).parent().parent().find('.clsLabel').text() == undefined) ? 0 : $(this).parent().parent().find('.clsLabel').text();
            var tSTC = $("#btotalTradedSTC").html();
            $("#btotalTradedSTC").html(parseFloat(parseFloat(cSTC) + parseFloat(tSTC)).toFixed(2));

        }
        else {
            chkCount--;
            $('#chkAll').prop('checked', this.checked)
            var cTotal = ($(this).parent().parent().find('.lblTotal').text() == '' || $(this).parent().parent().find('.lblTotal').text() == undefined) ? 0 : $(this).parent().parent().find('.lblTotal').text();
            var tSelsectedAmount = $("#lblTotalAmountSelected").html();
            $("#lblTotalAmountSelected").html(parseFloat(parseFloat(tSelsectedAmount) - parseFloat(cTotal)).toFixed(2));

            var cSTC = ($(this).parent().parent().find('.clsLabel').text() == '' || $(this).parent().parent().find('.clsLabel').text() == undefined) ? 0 : $(this).parent().parent().find('.clsLabel').text();
            var tSTC = $("#btotalTradedSTC").html();
            $("#btotalTradedSTC").html(parseFloat(parseFloat(tSTC) - parseFloat(cSTC)).toFixed(2));
        }
    });

    //$('#ResellerId').val(localStorage.getItem('PeakPay_Reseller'));

    var resellerId;
    var solarCompanyId;

    $('#importCSV').fileupload({

        url: urlImportCSV,
        dataType: 'json',
        //add: function (e, data) {
        //    resellerId = $("#ResellerId").val();
        //    data.formData = { resellerId: resellerId, solarCompanyId: solarCompanyId };
        //    data.submit();
        //},
        done: function (e, data) {
            if (data.result.status) {
                $("#datatable").dataTable().fnDraw();
                showSuccessMessage("CSV File has been imported successfully.");
            }
            else {
                if (data.result.error)
                    showErrorMessage(data.result.error);
                else
                    showErrorMessage("CSV File has not been imported.");
            }

        },

        progressall: function (e, data) {
        },

        singleFileUploads: false,
        send: function (e, data) {

            var mimeType = data.files[0].type;
            if (data.files.length == 1) {
                for (var i = 0; i < data.files.length; i++) {
                    if (data.files[i].name.length > 50) {
                        showErrorMessage("Please upload small filename.");
                        $('html').animate({ scrollTop: 0 }, 'slow');//IE, FF
                        $('body').animate({ scrollTop: 0 }, 'slow');
                        return false;
                    } else if (CheckSpecialCharInFileName(data.files[i].name)) {
                        showErrorMessage("Please upload file that not conatain <> ^ [] .");
                        $('html').animate({ scrollTop: 0 }, 'slow');//IE, FF
                        $('body').animate({ scrollTop: 0 }, 'slow');
                        return false;
                    }
                }
            }
            if (data.files.length > 1) {
                for (var i = 0; i < data.files.length; i++) {
                    if (data.files[i].size > parseInt(MaxImageSize)) {
                        showErrorMessage(" " + data.files[i].name + " Maximum file size limit exceeded. Please upload a file smaller  than" + " " + maxLogoSize + "MB");
                        return false;
                    } else if (CheckSpecialCharInFileName(data.files[i].name)) {
                        showErrorMessage("Please upload file that not conatain <> ^ [] .");
                        return false;
                    }
                }
            }
            else {
                if (data.files[0].size > parseInt(MaxImageSize)) {
                    showErrorMessage("Maximum  file size limit exceeded.Please upload a  file smaller than" + " " + maxLogoSize + "MB");
                    return false;
                }
            }
            if (!(mimeType == "text/csv" || mimeType == "application/csv" || mimeType == "csv" || mimeType == "text/x-csv" || mimeType == "application/x-csv" || mimeType == "text/x-comma-separated-values" ||
                mimeType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" || mimeType == "application/vnd.ms-excel" || mimeType == "text/comma-separated-values" || mimeType == '')) {
                showErrorMessage("Please upload a file with .csv extension.");
                return false;
            }

            $(".alert").hide();
            $("#errorMsgRegionForPopUp").html("");
            $("#errorMsgRegionForPopUp").hide();
            $('<input type="hidden">').attr({
                name: 'Guid',
                value: USERID,
                id: USERID,
            }).appendTo('form');
            return true;
        },
        //formData: { resellerId: resellerId, solarCompanyId: solarCompanyId},
        change: function (e, data) {
            $("#uploadFile").val("C:\\fakepath\\" + data.files[0].name);
        }
    }).prop('disabled', !$.support.fileInput).parent().addClass($.support.fileInput ? undefined : 'disabled');

});

$('#ResellerId').change(function () {
    BindSolarCompany($("#ResellerId").val(), 0);
    GetPeakPayJobStageCount();
    $("#datatable").dataTable().fnDraw();
})

function BindSolarCompany(rId, ramId) {
    var IsCompanyExist = false;
    $("#searchSolarCompany").val("");
    var scurl = '';
    var searchid = '';
    if (ramId == 0) {
        scurl = urlGetSolarCompanyByResellerId
        searchid = rId
    }
    else {
        scurl = urlGetSolarCompanyByRamId;
        searchid = ramId
    }

    $.ajax({
        type: 'POST',
        url: scurl,
        dataType: 'json',
        async: false,
        data: { id: searchid },
        success: function (solarcompany) {
            solarCompanyList = [];
            solarCompanyList.push({ value: '0', text: 'All' });
            $.each(solarcompany, function (i, company) {
                solarCompanyList.push({ value: company.Value, text: company.Text });
                if (IsCompanyExist == false && company.Value == localStorage.getItem('PeakPay_SolarCompanyId')) {
                    IsCompanyExist = true;
                }
            });
            var solarCompanyID = PeakPay_SolarCompanyId;
            if (solarCompanyID == '' || solarCompanyID == null) {
                if (IsCompanyExist) {
                    $('#hdnsolarCompanyid').val(localStorage.getItem('PeakPay_SolarCompanyId'));
                }
                else {
                    $('#hdnsolarCompanyid').val(solarCompanyList.length > 0 ? solarCompanyList[0].value : 0);
                    localStorage.setItem('PeakPay_SolarCompanyId', $('#hdnsolarCompanyid').val());
                }

                if (localStorage.getItem('PeakPay_SolarCompanyId') == '') {
                    $('#hdnsolarCompanyid').val(solarCompanyList.length > 0 ? solarCompanyList[0].value : 0);
                    localStorage.setItem('PeakPay_SolarCompanyId', $('#hdnsolarCompanyid').val());
                }

                $.each(solarCompanyList, function (key, value) {
                    if (value.value == localStorage.getItem('PeakPay_SolarCompanyId')) {
                        $("#searchSolarCompany").val(value.text);
                        $('#hdnsolarCompanyid').val(localStorage.getItem('PeakPay_SolarCompanyId'));
                    }
                });
            }
            else {
                $('#hdnsolarCompanyid').val(solarCompanyID);
            }
            //GetPeakPayJobStageCount(rId);
            //$("#datatable").dataTable().fnDraw();
        },
        error: function (ex) {
            alert('Failed to retrieve Solar Companies.' + ex);
        }
    });
    return false;
}

function SearchPeakPayRecords() {
    if (UserTypeID == 1 || UserTypeID == 3) {
        localStorage.setItem('PeakPay_Reseller', $('#ResellerId').val());
    }
    if (UserTypeID == 1 || UserTypeID == 3 || UserTypeID == 2 || UserTypeID == 5) {
        localStorage.setItem('PeakPay_solarcompanyId', $('#hdnsolarCompanyid').val());
    }

    localStorage.setItem('PeakPay_searchText', $("#txtSearchfor").val());
    localStorage.setItem('PeakPay_stcFromtcPrice', $("#txtStcFromPrice").val());
    localStorage.setItem('PeakPay_stcToPrice', $("#txtStcToPrice").val());
    localStorage.setItem('PeakPay_cerApprovedFromDate', $("#txtCERApprovedFromDate").val());
    localStorage.setItem('PeakPay_cerApprovedToDate', $("#txtCERApprovedToDate").val());
    localStorage.setItem('PeakPay_SettleBeforeFromDate', $("#txtSettleBeforeFromDate").val());
    localStorage.setItem('PeakPay_SettleBeforeToDate', $("#txtSettleBeforeToDate").val());
    localStorage.setItem('PeakPay_paymentFromDate', $("#txtPaymentFromDate").val());
    localStorage.setItem('PeakPay_paymentToDate', $("#txtPaymentToDate").val());
    localStorage.setItem('PeakPay_systSize', $("#SystemSize").val());

    GetPeakPayJobStageCount();
    $("#datatable").dataTable().fnDraw();
}

function ResetSearchFilters() {
    if (UserTypeID == 1 || UserTypeID == 3) {
        $("#ResellerId").val(localStorage.getItem('PeakPay_Reseller'));
    }
    if (UserTypeID == 1 || UserTypeID == 3 || UserTypeID == 2 || UserTypeID == 5) {
        localStorage.setItem('PeakPay_solarcompanyId', 0);
        $('#hdnsolarCompanyid').val(0);
    }

    $("#txtSearchfor").val('');
    $("#txtStcFromPrice").val('')
    $("#txtStcToPrice").val('')
    $("#txtCERApprovedFromDate").val('');
    $("#txtCERApprovedToDate").val('');
    $("#txtSettleBeforeFromDate").val('');
    $("#txtSettleBeforeToDate").val('');
    $("#txtPaymentFromDate").val('');
    $("#txtPaymentToDate").val('');
    $("#SystemSize").val('');

    document.getElementById('chkSentInvoice').checked = false;
    document.getElementById('chkUnsentInvoice').checked = true;
    document.getElementById('chkReadytoSTCInvoice').checked = true;

    var a = document.getElementById("peakPayStcstage_" + SelectedStageId);
    a.style.backgroundColor = "#f7f7f7";

    GetPeakPayJobStageCount();
    $("#datatable").dataTable().fnDraw();
}

function FromToDate(objFromDate, objToDate) {
    objFromDate.datepicker({
        format: dateFormat,
        autoclose: true
    }).on('changeDate', function () {
        if (objToDate.val() != '') {
            var fromDate = new Date(ConvertDateToTick(objFromDate.val(), dateFormat));
            var toDate = new Date(ConvertDateToTick(objToDate.val(), dateFormat));
            if (fromDate > toDate) {
                objToDate.val('');
            }
        }
        var tickStartDate = ConvertDateToTick(objFromDate.val(), dateFormat);
        tDate = moment(tickStartDate).format("MM/DD/YYYY");
        if (objToDate.data('datepicker')) {
            objToDate.data('datepicker').setStartDate(new Date(tDate));
        }
    });
    objToDate.datepicker({
        format: dateFormat,
        autoclose: true
    });
    objToDate.change(function () {
        var fromDate = new Date(ConvertDateToTick(objFromDate.val(), dateFormat));
        var toDate = new Date(ConvertDateToTick(objToDate.val(), dateFormat));
    });
}

function showErrorMessage(message) {
    $(".alert").hide();
    $("#successMsgRegion").hide();
    $("#errorMsgRegion").html(closeButton + message);
    $("#errorMsgRegion").show();
    $('html').animate({ scrollTop: 0 }, 'slow');//IE, FF
    $('body').animate({ scrollTop: 0 }, 'slow');
}

function showSuccessMessage(message) {
    $(".alert").hide();
    $("#errorMsgRegion").hide();
    $("#successMsgRegion").html(closeButton + message);
    $("#successMsgRegion").show();
    $('html').animate({ scrollTop: 0 }, 'slow');//IE, FF
    $('body').animate({ scrollTop: 0 }, 'slow');
}

function ExportCsv() {
    selectedRows = [];
    $('#datatable tbody input[type="checkbox"]').each(function () {
        if ($(this).prop('checked') == true) {

            var JobID = $(this).attr('jobid');
            selectedRows.push(JobID);

        }
    });
    if (selectedRows != null && selectedRows.length > 0) {
        window.location.href = urlExportCSV + selectedRows.join(',');

    }
    else {
        alert('Please select any job to export.');
    }
}

function SendSTCInvoice(IsSTCInvoice) {

    selectedRows = [];
    var IsSuccess = true;
    $('#datatable tbody input[type="checkbox"]').each(function () {
        if ($(this).prop('checked') == true) {
            var PWDSWHCode = $(this).attr('stcPvdCode');
            var IsInvoiced = $(this).attr('isInvoiced');
            //var IsCreditNote = $(this).attr('IsCreditNote');          
            var stcStatusId = $(this).attr('stcStatusId');
            var stcPrice = $(this).attr('stcPrice');
            var stcInvoiceCount = $(this).attr('stcinvoicecnt');

            if (stcPrice == 0) {
                alert('Please set price');
                IsSuccess = false;
            }
            else if (PWDSWHCode == '' || PWDSWHCode == undefined) {
                alert("Can't generate STCInvoice for STC record which has not PVD code.")
                IsSuccess = false;
            }
            //else if (stcStatusId != 22) {
            //    alert("Can't generate STCInvoice on Pending Approval Stc status")
            //    IsSuccess = false;
            //}
            else if (stcInvoiceCount > 0) {
                alert("STCInvoice has been already generated for this job.")
                IsSuccess = false;
            }
            else if (IsInvoiced == '1' && IsSTCInvoice == '1') {
                alert("STCInvoice has been already generated for this job.")
                IsSuccess = false;
            }
                //else if (IsCreditNote == "true" && IsSTCInvoice == '0') {
                //    alert("CreditNote has been already generated for this job.")
                //    IsSuccess = false;
                //}
                //else if (IsInvoiced == 'false' && IsSTCInvoice == '0') {
                //    alert("First generate STC Invoice then after credit note will be generated.")
                //    IsSuccess = false;
                //}
            else {
                var STCJobDetailsID = $(this).attr('stcjobdetailsid');
                var STCTerm = $(this).attr('settlementTerm');
                var jobid = $(this).attr("jobid");
                selectedRows.push(STCJobDetailsID + '_' + STCTerm + '_' + jobid);
            }
        }
    })

    if (IsSuccess) {

        if (selectedRows != null && selectedRows.length > 0) {
            $.ajax({
                url: urlGenerateSTCInvoiceForSelectedJobs,
                type: "POST",
                //async: false,
                data: JSON.stringify({ resellerId: $('#ResellerId').val(), jobs: selectedRows.join(','), isstcinvoice: IsSTCInvoice, solarCompanyId: $('#hdnsolarCompanyid').val() }),
                dataType: "json",
                contentType: "application/json; charset=utf-8",
                success: function (data) {

                    if (data && data.status == false) {
                        if (data.error.toLowerCase() == 'specified method is not supported.' || data.error.toLowerCase() == 'renewtokenexception') {
                            window.open(urlConnect, "_blank");
                        }
                        else if (data.error.toLowerCase() == 'sessiontimeout')
                            window.location.href = urlLogout;
                    }
                    if (data.success) {
                        showSuccessMessage('Invoice is generated successfully for selected jobs.');
                        $("#datatable").dataTable().fnDraw();
                    }
                },
                error: function (data) {
                    alert('error');
                }
            });
        }
        else {
            alert('Please select any record to generate STC Invoice.');
        }
    }
}

function BulkChangeInvoiceStatus() {

    if (document.getElementById('STCJobStageID').selectedIndex > 0) {
        var confirmMsg = '';
        selectedRows = [];
        var IsValid = true;

        $('#datatable tbody input[type="checkbox"]').each(function () {
            if ($(this).prop('checked') == true) {
                if ($(this).attr('stcStatusId') != 22) {
                    selectedRows.push($(this).attr('stcjobdetailsid'));
                }
                else {
                    IsValid = false;
                    return false;
                }
            }
        })

        if (IsValid) {
            if (selectedRows != null && selectedRows.length > 0) {
                if (confirmMsg == '') {
                    confirmMsg = document.getElementById('STCJobStageID').value == 22 ? "Are you sure you want to change stc status  as CERApproved of selected jobs as you can not reverse it after change ?" : "Are you sure you want to change stc status of selected jobs ?"
                }
                var result = confirm(confirmMsg);
                if (result) {
                    $.ajax({
                        type: 'POST',
                        url: urlBulkChangeSTCJobStage,
                        async: false,
                        dataType: 'json',
                        contentType: 'application/json; charset=utf-8',
                        data: JSON.stringify({ stcjobstage: document.getElementById('STCJobStageID').value, stcjobids: selectedRows.join(',') }),
                        success: function (data) {
                            if (data.success) {
                                showSuccessMessage("Job Stages updated successfully.")
                                $("#datatable").dataTable().fnDraw();
                                document.getElementById('STCJobStageID').selectedIndex = 0;
                            }
                        },
                        error: function (ex) {
                            showErrorMessage("Job Stages cannot be updated");
                        }
                    });
                }
            }
            else {
                alert('Please select atleast one Job to update STC Stage.');
                return false;
            }
        }
        else {
            alert('You can not change stc status of job which have CER Approved status.');
            return false;
        }
    }
    else {
        alert('Please select STC Job Stage first.');
        return false;
    }
};

function SetStcPrice() {

    selectedRows = [];
    var IsValid = true;
    var IsSTCPrice = true;
    var result = true;

    $('#datatable tbody input[type="checkbox"]').each(function () {
        if ($(this).prop('checked') == true) {

            var STCJobDetailsID = $(this).attr('stcjobdetailsid');
            selectedRows.push(STCJobDetailsID);
            IsValid = true;

            var stcPrice = $(this).attr('stcPrice');
            if (stcPrice > 0) {
                IsSTCPrice = false;
            }
            if ($(this).attr('isinvoiced') == 1) {
                alert('Cannot change stc price of jobs whose invoice is already generated.');
                IsValid = false;
                return false;
            }
            //else {
            //    var STCJobDetailsID = $(this).attr('stcjobdetailsid');
            //    selectedRows.push(STCJobDetailsID);
            //}           
        }
    })

    if (IsValid) {
        if (!IsSTCPrice) {
            result = confirm('STC price already exists, are you sure you want to change STC price.');
        }
        if (result) {
            if ($('#SetSTCPrice').val() > 0) {
                if (selectedRows != null && selectedRows.length > 0) {
                    $.ajax({
                        type: 'POST',
                        url: urlPeakPaySetStcPrice,
                        async: false,
                        dataType: 'json',
                        contentType: 'application/json; charset=utf-8',
                        data: JSON.stringify({ stcPrice: $('#SetSTCPrice').val(), stcJobDetailIds: selectedRows.join(',') }),
                        success: function (data) {
                            if (data.success) {
                                showSuccessMessage("Stc Price set successfully.")
                                $("#datatable").dataTable().fnDraw();
                                $('#STCPrice').val(0);
                            }
                        },
                        error: function (ex) {
                            showErrorMessage("Cannot set Stc Price");
                        }
                    });
                }
                else {
                    alert('Please select atleast one Job to set STC Price.');
                    return false;
                }
            }
            else {
                alert('Please enter STC Price.');
                return false;
            }
        }
    }
};

function ChangePeakPayInvoiceStatus(invoiceStatus) {
    selectedRows = [];
    var IsValid = true;

    $('#datatable tbody input[type="checkbox"]').each(function () {
        if ($(this).prop('checked') == true) {
            if (invoiceStatus == 3 && ($(this).attr('stcstatusid') != 22 || $(this).attr('stcprice') == 0)) {
                alert('Jobs with Stc status as pending approval and STC price less than 0 cannot be marked as ReadyToSent invoice.');
                IsValid = false;
                return false;
            }
            if ($(this).attr('isinvoiced') == 1) {
                alert('Cannot change invoice status of jobs whose invoice is already generated.');
                IsValid = false;
                return false;
            }
            else {
                var STCJobDetailsID = $(this).attr('stcjobdetailsid');
                selectedRows.push(STCJobDetailsID);
            }
        }
    })

    if (IsValid) {
        if (selectedRows != null && selectedRows.length > 0) {
            $.ajax({
                type: 'POST',
                url: urlChangePeakpayInvoiceStatus,
                async: false,
                dataType: 'json',
                contentType: 'application/json; charset=utf-8',
                data: JSON.stringify({ stcJobDetailIds: selectedRows.join(','), invoiceStatus: invoiceStatus }),
                success: function (data) {
                    if (data.success) {
                        if (data.InvoiceStatus == 2)
                            showSuccessMessage("PeakPay job is marked as unsent.")
                        if (data.InvoiceStatus == 3)
                            showSuccessMessage("PeakPay job is marked as readyToSent.")

                        GetPeakPayJobStageCount();
                        $("#datatable").dataTable().fnDraw();
                    }
                },
                error: function (ex) {
                    showErrorMessage("Cannot change invoice status");
                }
            });
        }
        else {
            alert('Please select atleast one Job to change invoice status.');
            return false;
        }
    }
};

function SetStageId(id) {
    var a = document.getElementById("peakPayStcstage_" + SelectedStageId);
    a.style.backgroundColor = "#f7f7f7";

    SelectedStageId = id;

    var a = document.getElementById("peakPayStcstage_" + SelectedStageId);
    a.style.backgroundColor = "#686868";

    $("#txtSearchfor").val('');
    $("#txtStcFromPrice").val('')
    $("#txtStcToPrice").val('')
    $("#txtCERApprovedFromDate").val('');
    $("#txtCERApprovedToDate").val('');
    $("#txtSettleBeforeFromDate").val('');
    $("#txtSettleBeforeToDate").val('');
    $("#txtPaymentFromDate").val('');
    $("#txtPaymentToDate").val('');
    $("#SystemSize").val('');

    document.getElementById('chkSentInvoice').checked = false;
    document.getElementById('chkUnsentInvoice').checked = true;
    document.getElementById('chkReadytoSTCInvoice').checked = true;

    GetPeakPayJobStageCount();
    $("#datatable").dataTable().fnDraw();
}

function GetPeakPayJobStageCount(rid) {

    var resellerId = 0;
    var ramId = 0;
    var solarcompanyId = 0;

    if (UserTypeID == 1 || UserTypeID == 3) {
        resellerId = (rid == 0 || typeof (rid) == 'undefined') ? $('#ResellerId').val() : rid;
        solarcompanyId = $('#hdnsolarCompanyid').val();
    }
    else if (UserTypeID == 2) {
        resellerId = ProjectSession_ResellerId;
        //ramId = document.getElementById("RamId").value;       
        solarcompanyId = $('#hdnsolarCompanyid').val();
    }
    else if (UserTypeID == 5) {
        resellerId = ProjectSession_ResellerId;
        //ramId = LoggedInUserId;        
        solarcompanyId = $('#hdnsolarCompanyid').val();
    }
    else {
        solarcompanyId = ProjectSession_SolarCompanyId;
    }

    $.ajax({
        type: 'POST',
        url: urlGetPeakPayJobStageCount,
        dataType: 'json',
        async: false,
        data: { reseller: resellerId, ram: ramId, sId: solarcompanyId, isAllScaJobView: isAllScaJobView },
        success: function (jobstagescount) {
            var sum = 0;
            $.each(jobstagescount.lstPeakPayJobStagesCount, function (i, count) {
                document.getElementById("peakPayStcstage_" + count.JobStageId).innerHTML = count.StageName + '<span>(' + count.jobCount + ')</span>';
                sum = sum + count.jobCount;
            });
            document.getElementById("peakPayStcstage_0").innerHTML = 'Show All' + '<span>(' + sum + ')</span>';
        },
        error: function (ex) {
            alert('Failed to retrieve count for Job Stages.' + ex);
        }
    });
    return false;
}
