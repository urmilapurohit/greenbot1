var VEECBaselineAreaId;
var PreName;
var  spacetype = $('#spacetype'),
        bcaclassification = $('#bcaclassification'),
        LampBallastCombination = $('#LampBallastCombination'),
        LampCategory = $('#LampCategory'),
        BaselineAssetLifetimeReference = $('#BaselineAssetLifetimeReference'),
        UpgradeAssetLifetimeReference = $('#UpgradeAssetLifetimeReference'),
        ProductBrand = $('#ProductBrand'),
        ProductModel = $('#ProductModel'),
        VRUProductBrand = $('#VRUProductBrand'),
        VRUProductModel = $('#VRUProductModel'),
        FirstController = $('#FirstController'),
        SecondController = $('#SecondController');
AreaSelectIdBaseline = $('#AreaSelectIdBaseline'),
AreaSelectIdUpgrade = $('#AreaSelectIdUpgrade');

$(document).ready(function () {
    $('#btnAddArea').click(function () {
       
        $('#popupAddEditArea').modal({ backdrop: 'static', keyboard: false });
        $("#btnCancelEditAreaModel").hide();

    })
    dropDownData.push({ id: 'spacetype', key: "", value: spacetype, hasSelect: true, callback: null, defaultText: null, proc: 'VEEC_GetSpaceType', param: [], bText: 'Name', bValue: 'Id' },
                    { id: 'bcaclassification', key: "", value: bcaclassification, hasSelect: true, callback: null, defaultText: null, proc: 'VEEC_GetBCAClassification', param: [], bText: 'Name', bValue: 'Id' },
                    { id: 'LampBallastCombination', key: "", value: LampBallastCombination, hasSelect: true, callback: null, defaultText: null, proc: 'VEEC_GetLampBallastCombination', param: [], bText: 'Name', bValue: 'Id' },
                    { id: 'LampCategory', key: "", value: LampCategory, hasSelect: true, callback: null, defaultText: null, proc: 'VEEC_GetLampCategory', param: [], bText: 'Name', bValue: 'Id' },
                    { id: 'BaselineAssetLifetimeReference', key: "", value: BaselineAssetLifetimeReference, hasSelect: true, callback: null, defaultText: null, proc: 'VEEC_GetBaselineAssetLifetimeReference', param: [], bText: 'Name', bValue: 'Id' },
                    { id: 'UpgradeAssetLifetimeReference', key: "", value: UpgradeAssetLifetimeReference, hasSelect: true, callback: null, defaultText: null, proc: 'VEEC_GetUpgradeAssetLifetimeReference', param: [], bText: 'Name', bValue: 'Id' },
                    { id: 'ProductBrand', key: "", value: ProductBrand, hasSelect: true, callback: null, defaultText: null, proc: 'VEEC_GetProductBrand', param: [], bText: 'Name', bValue: 'Name' },
                    { id: 'VRUProductBrand', key: "", value: VRUProductBrand, hasSelect: true, callback: null, defaultText: null, proc: 'VEEC_GetVRUProductBrand', param: [], bText: 'Name', bValue: 'Id' },
                    { id: 'VRUProductModel', key: "", value: VRUProductModel, hasSelect: true, callback: null, defaultText: null, proc: 'VEEC_GetVRUProductModel', param: [], bText: 'Name', bValue: 'Id' },
                    { id: 'FirstController', key: "", value: FirstController, hasSelect: true, callback: null, defaultText: null, proc: 'VEEC_GetFirstControllerType', param: [], bText: 'Name', bValue: 'Id' },
                    { id: 'SecondController', key: "", value: SecondController, hasSelect: true, callback: null, defaultText: null, proc: 'VEEC_GetSecondControllerType', param: [], bText: 'Name', bValue: 'Id' },
                    { id: 'AreaSelectIdBaseline', key: "", value: AreaSelectIdBaseline, hasSelect: true, callback: AreaNameforSelectOption, defaultText: null, proc: 'VEEC_GetVEECAreaName', param: [{ VEECId: VEECID }], bText: 'Name', bValue: 'VEECAreaId' });
 
    //{ id: 'AreaSelectIdUpgrade', key: "", value: AreaSelectIdUpgrade, hasSelect: true, callback: AreaNameforSelectOption, defaultText: null, proc: 'VEEC_GetVEECAreaName', param: [{ VEECId: VEECID }], bText: 'Name', bValue: 'VEECAreaId' });


    if (VEECDetail_ScoIDVEEC != 0 && VEECDetail_ScoIDVEEC > 0) {
        FillDropDown('ScoIDVEEC', urlGetSCOUser, VEECDetail_ScoIDVEEC, true, null);
        //dropDownData.push({ id: 'ScoIDVEEC', key: "", value: VEECDetail_ScoIDVEEC, hasSelect: true, callback: null, defaultText: null, proc: 'User_GetSCOUser', param: [], bText: 'Name', bValue: 'UserId' });
    }
    else {
        FillDropDown('ScoIDVEEC', urlGetSCOUser, null, true, null);
        //dropDownData.push({ id: 'ScoIDVEEC', key: "", value: null, hasSelect: true, callback: null, defaultText: null, proc: 'User_GetSCOUser', param: [], bText: 'Name', bValue: 'UserId' });

    }


    dropDownData.bindDropdown();
    $("#BaselineAssetLifetimeReference").change(function () {
        var BaselineAssetLifetimeReferencetype = $(this).val();
        if (BaselineAssetLifetimeReferencetype == 1 || BaselineAssetLifetimeReferencetype == 5 || BaselineAssetLifetimeReferencetype == 6) {
            $(".rlh").show();
            $("#RatedLifetimeHours").rules("add", {
                required: true,
            });
            $("#RatedLifetimeHoursLabel").addClass("required")
        } else {
            $("#RatedLifetimeHours").rules("add", {
                required: false,
            });
            $("#RatedLifetimeHoursLabel").removeClass("required")
            $(".rlh").hide();
        }
        if (BaselineAssetLifetimeReferencetype == 2) {
            $(".toc").hide();
        } else {
            $(".toc").show();
        }
    })
    $("#LampType").change(function () { 
        if ($(this).val() == 2) {
            $(".lbc").hide()
            $("#LampBallastCombination").rules("add", {
                required: false,
            });
            $("#LampBallastCombinationLabel").removeClass("required")
        } else {
            $(".lbc").show()
            $("#LampBallastCombination").rules("add", {
                required: true,
            });
            $("#LampBallastCombinationLabel").addClass("required")
        }
        hideShowProduct();
        showhideRatedHourOrLampPower();
    })
    $("#spacetype").change(function () {
        if ($(this).val() == 26) {
            $(".stu").show()
            $("#spacetypeunlisted").rules("add", {
                required: true,
            });
            $("#spacetypeunlistedlabel").addClass("required")
        } else {
            $(".stu").hide()
            $("#spacetypeunlisted").rules("add", {
                required: false,
            });
            $("#spacetypeunlistedlabel").removeClass("required")
        }
        $.ajax(
         {
             url: showHideBCAClassificationOnSpaceTypeChange,
             dataType: 'json',
             contentType: 'application/json; charset=utf-8', // Not to set any content header
             type: 'post',
             data: JSON.stringify({ spaceTypeId: $("#spacetype").val()}),
             success: function (data) {

                 if (data != 1) {
                     $(".bca").show();
                     $("#bcaclassification").rules("add", {
                         required: true,
                     });
                     $("#bcaclassificationLabel").addClass("required")
                 }
                 else {
                     $(".bca").hide();
                     $("#bcaclassification").rules("add", {
                         required: false,
                     });
                     $("#bcaclassificationLabel").removeClass("required")
                 }
             },
             error: function () {

             }
         });
    })
    $("#UpgradeAssetLifetimeReference").change(function () {
        hideShowProduct();
        typeControllerValidate();
        showhideRatedHourOrLampPower();
    })
    $("#FirstController").rules("add", {
        required: false,
    });
    $("#spacetypeunlisted").rules("add", {
        required: false,
    });
    $("#spacetypeunlistedlabel").removeClass("required")
    $("#SecondController").rules("add", {
        required: false,
    });
    $("#NominalLampPower").rules("add", {
        required: false,
    });
    $("#NominalLampPowerLabel").removeClass("required")
    $("#RatedLifetimeHours").rules("add", {
        required: false,
    });
    $("#RatedLifetimeHoursLabel").removeClass("required")
    $("#ProductBrand").rules("add", {
        required: false,
    });
    $("#ProductBrandLabel").removeClass("required")
    $("#ProductModel").rules("add", {
        required: false,
    });
    $("#ProductModelLabel").removeClass("required")
    $('#popupAddBaseLine').on('hidden.bs.modal', function () {
        $("#AddBaselineEquipment").find(".form-control").each(function () {
            $(this).removeClass("input-validation-error")
        });

    })
    $('#AreaSelectIdBaseline').change(function () {
        if ($('#AreaSelectIdBaseline option:selected').text() == "Select") {
            showMessageForVEEC("Please Select Area Name For Baseline Equipment.",true,"","errormessageforselectarea")
        }
        GetBaselineUpgradeData();
    });
    $("#ProductBrand").change(function () {
        var productBrand = $(this).val();
        FillDropDown('ProductModel', urlGetProductmodel + "?ProductBrand=" + productBrand, "", true, null);
    })
    $('#btnAddBaselineEquipment').click(function () {
       
        
        var jsonData = $('#AddBaselineEquipment').serializeToJson();
        if ($('#baselineupgrade').val() == "Baseline") {
            jsonData.BaselineEquipment.BaselineUpgrade = 1;
        } else {
            jsonData.BaselineEquipment.BaselineUpgrade = 2
        }
        if (jsonData.BaselineEquipment.HVACAC == "Yes") {
            jsonData.BaselineEquipment.HVACAC = true;
        } else {
            jsonData.BaselineEquipment.HVACAC = false;
        }
        jsonData.BaselineEquipment.Lampcategory = $("#LampCategory").val();
        
        var VEECId = VEECID;

        var VEECAreaId = $('#AreaSelectIdBaseline').val();
        

        url = urlAddBaselineEquipment;
        if ($('#AddBaselineEquipment').valid()) {
            //var requiredData = []
            //$("#AddBaselineEquipment").find(".required").each(function () {
            //    requiredData.push($(this).next().attr("name"));
            //});

            var requiredData = [], newdata = [];
            $("#AddBaselineEquipment").find(".required").each(function () {
                requiredData.push($(this).next().attr("name"));
            });
            $("#AddBaselineEquipment").find(".form-control").each(function () {
                
                if (!requiredData.includes($(this).attr("name"))) {
                    newdata.push($(this).attr("name"));
                }
            });

            $.ajax(
          {
              url: url + VEECId + '&VEECAreaId=' + VEECAreaId,
              dataType: 'json',
              contentType: 'application/json; charset=utf-8', // Not to set any content header
              type: 'post',
              data: JSON.stringify({ baselineequipment: jsonData.BaselineEquipment, requiredData: newdata }),
              success: function (data) {
                 
                  if (data.status == true) {
                      GetBaselineUpgradeData();

                      $('#popupAddBaseLine').modal('toggle');
                    

                      if (data.save == "Baseline") {
                          showMessageForVEEC("BaselineEquipment saved successfully.",false,'successmessageforbaselineequipment','successmessageforaddbaseline')
                          
                      }
                      else {
                          showMessageForVEEC("UpgradeEquipment saved successfully.", false, 'successmessageforbaselineequipment', 'successmessageforaddbaseline')
                      }
                  }
                  else {
                      showMessageForVEEC(data.errorMessage, true, 'successmessageforbaselineequipment', 'successmessageforaddbaseline')
                  }
              },
              error: function () {
                 
              }
          });
        }

    });
    
   
    
    $('#AreaNameDatatable').DataTable({
        'iDisplayLength': 10,
        'paginate': false,
        //lengthMenu:@ProjectConfiguration.GetPageSize,
        'language': {
            sLengthMenu: "Page Size: _MENU_"
        },
        'columns': [
                {
                    'data': 'Name',
                    'orderable': true
                },
                {
                    'data': 'VEECAreaId',
                    "render": function (data, type, full, meta) {
                        imgedit = 'background:url(../Images/edit-icon.png) no-repeat center; text-decoration:none;';
                        imgdelete = 'background:url(../Images/delete-icon.png) no-repeat center; text-decoration:none;';
                        var editHref = '';
                        var deleteHref = '';


                        editHref = "javascript:UpdateAreaName('" + full.VEECAreaId + "','" + full.Name + "')";
                        deleteHref = "javascript:DeleteAreaName('" + full.VEECAreaId + "')";
                        editButton = '&nbsp;&nbsp;' + '<a href="' + editHref + '" class="edit sprite-img"  title="Accept">' + '&nbsp; &nbsp; &nbsp; &nbsp;' + '</a>';
                        deleteButton = '&nbsp;&nbsp;' + '<a href="' + deleteHref + '" class="delete sprite-img"  title="Accept">' + '&nbsp; &nbsp; &nbsp; &nbsp;' + '</a>';




                        return editButton + deleteButton;
                    },
                    "orderable": false
                },
        ],
        'dom': "<<'table-responsive tbfix't><'paging grid-bottom prevar'p><'bottom'l><'clear'>>",
        'initComplete': function (settings, json) {
            $('.grid-bottom span:first').attr('id', 'spanMain');
            $("#spanMain span").html("");
            $(".ellipsis").html("...");
        },
        'bServerSide': true,
        'sAjaxSource': urlGetAreaNameRecords,
        "fnDrawCallback": function () {
            $("#AreaNameDatatable_wrapper").find(".bottom").find(".dataTables_paginate").remove();
            $(".paging a.previous").html("&nbsp");
            $(".paging a.next").html("&nbsp");
            $('.grid-bottom span:first').attr('id', 'spanMain');
            $("#spanMain span").html("");
            $(".ellipsis").html("...");
            $(".popupgrid-bottom").hide();
            $("#AreaNameDatatable_length").hide();
            var table = $('#AreaNameDatatable').DataTable();
            $("#AreaNameDatatable tr td:nth-child(2)").addClass("action");
            
        },

        "fnServerParams": function (aoData) {
            aoData.push({ "name": "VEECId", "value": VEECID });
        }
    });


    $('#btnClearBaselineEquipment').click(function () {
        var baseLineUpgrade = $('#baselineupgrade').val();
        $('#AddBaselineEquipment')[0].reset();
        $('#baselineupgrade').val(baseLineUpgrade);

    });
    $("#btnCancelEditAreaModel").click(function () {
        $("#areaname").val('');
        $("#veecareaid").val('');
        //$('#btnAddAreaModel').html('<span class="sprite-img add_ic"></span>Add');
        $("#btnCancelEditAreaModel").hide();
    });

    $('#btnRearrangeCalcZone').click(function () {
        var VEECId = VEECID;
        var url = urlRearrangeCalcZone + VEECId;
        $.ajax(
          {
              url: url,
              dataType: 'json',
              contentType: 'application/json; charset=utf-8', // Not to set any content header
              type: 'post',
              success: function (data) {
                 
                  if (data.status == true) {
                      GetBaselineUpgradeData();
                      showMessageForVEEC(data.successMsg, false, "successmessageforbaselineequipment","")
                     
                  }
                  else {
                      showMessageForVEEC(data.errorMsg, true, "", "successmessageforaddbaseline")
                     
                  }
              },
              error: function () {

              }
          });
    });

    $('#btnAddAreaModel').click(function () {
        //var RegExpression = /^[a-zA-Z\s]*$/;
        //var RegExpression = /[^a-zA-Z0-9]/;

        if ($('#AddEditArea').valid()) {
            if ($('#areaname').val() != ""){
                var areaname = $("#areaname").val();
                var veecareaid = $("#veecareaid").val();
                var VEECId = VEECID;

               

                if (veecareaid == "") {
                    url = urlAddArea + areaname + '&VEECId=' + veecId + '&veecareaid=' + 0;
                }
                else {
                    url = urlEditArea + areaname + '&VEECId=' + veecId + '&veecareaid=' + veecareaid + '&veecpreareaName=' + PreName;
                }

                $.ajax({
                    url: url,
                    contentType: 'application/json',
                    method: 'post',
                    success: function (response) {
                        if (response == null) {
                            showMessageForVEEC("Same area name must exist.", true, "", "successmessageforaddarea")
                            
                        }
                        else {
                            showMessageForVEEC("Area Name saved successfully.", false, "successmessageforaddarea", "")
                            $("#AreaNameDatatable").dataTable().fnDraw();
                            $("#datatable").dataTable().fnDraw();
                            //$('#btnAddAreaModel').html('<span class="sprite-img add_ic"></span>Add');
                            $('#areaname').val('');
                            FillDropDown('AreaSelectIdBaseline', urlGetAreaNameList + VEECID, AreaSelectIdBaseline, true, AreaNameforSelectOption, null);
                            //FillDropDown('AreaSelectIdUpgrade', urlGetAreaNameList + VEECID, AreaSelectIdUpgrade, true, AreaNameforSelectOption, null);
                             $('#veecPhoto').empty();
                            $('#veecPhoto').append(response.veecPhotoView);
                        }

                    }

                });
            }
            else {
                showMessageForVEEC("Please Add Area Name", true, "", "successmessageforaddarea")
            }
        }
    });

})

function UpdateAreaName(VEECAreaId, Name) {

    PreName = Name;
    selectedVEECAreaId = VEECAreaId;
    $("#areaname").val(PreName);
    $("#veecareaid").val(selectedVEECAreaId);
    //$('#btnAddAreaModel').html('<span class="sprite-img edit_ic"></span>Edit');
    $("#btnCancelEditAreaModel").show();

}

function DeleteAreaName(VEECAreaId) {
    
    var result = confirm("Are you sure you want to delete this record ?");
    if (result) {
        if (parseFloat(VEECAreaId) > 0) {
            $.ajax({
                url: urlDeleteVEECAreaName,
                dataType: 'json',
                method: "post",
                data: { id: VEECAreaId, veecId: VEECID },
                success: function (data) {
                    if (data.success) {
                        showMessageForVEEC("Area Name Deleted Successfully.", false, "successmessageforaddarea", "")
                        $("#AreaNameDatatable").dataTable().fnDraw();
                        $("#datatable").dataTable().fnDraw();
                        VEECBaselineAreaId = undefined;
                        FillDropDown('AreaSelectIdBaseline', urlGetAreaNameList + VEECID, AreaSelectIdBaseline, true, AreaNameforSelectOption, null);
                        $('#veecPhoto').empty();
                        $('#veecPhoto').append(data.veecPhotoView);
                    }
                }
            });
            
        }
    }
}
function AreaNameforSelectOption() {
   
    if (VEECBaselineAreaId == undefined) {
        $("#AreaSelectIdBaseline").val($("#AreaSelectIdBaseline option:eq(1)").val());
    }
    if (VEECBaselineAreaId != undefined) {
        $("#AreaSelectIdBaseline").val(VEECBaselineAreaId);
    }
    $("#AreaSelectIdBaseline").append('<option value = "0" >All</option>')
    //GetBaselineUpgradeData();
      
}

function EquipmentValidations(baselineUpgrade) {
    if (baselineUpgrade == 1) {
        $(".baseline").show();
        $(".upgrade").hide();
        $('#LampCategory').val(1)
        var BaselineAssetLifetimeReferencetype = $("#BaselineAssetLifetimeReference").val();
        if (BaselineAssetLifetimeReferencetype == 1 || BaselineAssetLifetimeReferencetype == 5 || BaselineAssetLifetimeReferencetype == 6) {
            $(".rlh").show();
            $("#RatedLifetimeHours").rules("add", {
                required: true,
            });
            $("#RatedLifetimeHoursLabel").addClass("required")
        }
        if (BaselineAssetLifetimeReferencetype == 2) {
            $(".toc").hide();
        }
        $("#BaselineAssetLifetimeReference").rules("add", {
            required: true,
        });
        $("#BaselineAssetLifetimeReferenceLabel").addClass("required")
        $("#UpgradeAssetLifetimeReference").rules("add", {
            required: false,
        });
        $("#UpgradeAssetLifetimeReferenceLabel").removeClass("required")
        $("#LampType").rules("add", {
            required: false,
        });
        $("#LampTypeLabel").removeClass("required")
        $("#NominalLampPower").rules("add", {
            required: true,
        });
        $("#NominalLampPowerLabel").addClass("required")
        $("#ProductBrand").rules("add", {
            required: false,
        });
        $("#ProductBrandLabel").removeClass("required")
        $("#ProductModel").rules("add", {
            required: false,
        });
        $("#ProductModelLabel").removeClass("required")
        $(".nlp").show();
    } else if (baselineUpgrade == 2) {
        $(".baseline").hide();
        $(".upgrade").show();
        $('#LampCategory').val(2)
        var lamptype = $("#LampType").val();
        if (lamptype == 2) {
            $(".lbc").hide();
        }
        hideShowProduct();
        typeControllerValidate();
        showhideRatedHourOrLampPower();
        $("#UpgradeAssetLifetimeReference").rules("add", {
            required: true,
        });
        $("#UpgradeAssetLifetimeReferenceLabel").addClass("required")
        $("#LampType").rules("add", {
            required: true,
        });
        $("#LampTypeLabel").addClass("required")
        $("#BaselineAssetLifetimeReference").rules("add", {
            required: false,
        });
        $("#BaselineAssetLifetimeReferenceLabel").removeClass("required")
    }
}

function deleteBaselineUpgradeEquipmentDetail(BaselineUpgradeEquipmentId,BaselineUpgrade) {
   

    var result = confirm("Are you sure you want to delete this record ?");
    if (result) {
        if (parseFloat(BaselineUpgradeEquipmentId) > 0) {
            $.ajax({
                url: urlDeleteBaselineUpgradeEquipment,
                dataType: 'json',
                method: "post",
                data: { id: BaselineUpgradeEquipmentId },
                success: function (data) {
                    if (data.success) {
                        if (BaselineUpgrade == 1) {

                            $("#BaselineEquipmentDetail").find('[data-panelid=' + BaselineUpgradeEquipmentId + ']').remove();
                            showMessageForVEEC("BaselineEquipment Deleted Successfully.", false, "successmessageforbaselineequipment", "")
                           
                        } else if (BaselineUpgrade == 2) {
                            $("#UpgradelineEquipmentDetail").find('[data-panelid=' + BaselineUpgradeEquipmentId + ']').remove();
                            showMessageForVEEC("UpgradelineEquipment Deleted Successfully.", false, "successmessageforbaselineequipment", "")
                        }
                        
                    }
                }
            });
        }
    }
}
var productModel = "";
function showBaselineUpgradeEquipmentDetail(jsonData, BaselineUpgrade) {
  
   clearBaselineValues();
    var obj = $.parseJSON(jsonData);
    $('#spacetype').val(obj['Spacetype']);
    $('#spacetypeunlisted').val(obj['SpaceTypeUnlisted']);
    $('#bcaclassification').val(obj['BCAClassification']);
    if(BaselineUpgrade == 1){
        $('#baselineupgrade').val("Baseline");
    }else if(BaselineUpgrade == 2){
        $('#baselineupgrade').val("Upgrade");
        $("#LampType").val(obj['LampType'])
    }
    $("#baselineupgrade").attr("disabled", "disbled");
    $('#LampBallastCombination').val(obj['LampBallastCombination']);
    $('#LampCategory').val(obj['Lampcategory']);
    $('#Quantity').val(obj['Quantity']);
    $('#BaselineAssetLifetimeReference').val(obj['BaselineAssetLifetimeReference']);
    $('#UpgradeAssetLifetimeReference').val(obj['UpgradeAssetLifetimeReference']);
    $('#ProductBrand').val(obj['ProductBrand']);
    
    FillDropDown('ProductModel', urlGetProductmodel + "?ProductBrand=" + obj['ProductBrand'], "", true, selectProductModel);
    productModel = obj['ProductModel'];
    $('#RatedLifetimeHours').val(obj['RatedLifetimeHours']);
    $('#NominalLampPower').val(obj['NominalLampPower']);
    if (obj['TypeOfFirstController'] == 0) {
        $('#FirstController option:contains(Select)').attr('selected', 'selected');
    } else {
        $('#FirstController').val(obj['TypeOfFirstController'])
    }
    if (obj['TypeOfSecondController'] == 0) {
        $('#SecondController option:contains(Select)').attr('selected', 'selected');
    } else {
        $('#SecondController').val(obj['TypeOfSecondController'])
    }
    $('#VRUProductBrand').val(obj['VRUProductBrand']);
    $('#VRUProductModel').val(obj['VRUProductModel']);
    if (obj['HVACAC'] == true) {
        $('#HVAC').val("Yes");
    } else {
        $('#HVAC').val("No");
    }
    $('#Definition').val(obj['Definition']);
    $('#BaselineEquipmentId').val(obj['BaselineEquipmentId']);
    $('#popupAddBaseLine').modal({ backdrop: 'static', keyboard: false });
    
    EquipmentValidations(BaselineUpgrade);
}
function selectProductModel() {
    $('#ProductModel').val(productModel);
}
function GetBaselineUpgradeData() {
    var VEECId = VEECID;
    var id = $('#AreaSelectIdBaseline').val();
    $('#veecareaidforselectedarea').val(id);
    VEECBaselineAreaId = $('#veecareaidforselectedarea').val();
    
    $.ajax(
          {
              url: urlReloadBaselineEquipment,
              dataType: 'json',
              data: { id: VEECId, areaId: VEECBaselineAreaId },
              contentType: 'application/json; charset=utf-8',
              type: 'get',           
              success: function (response) {
                  
                  if (response != null) {
                      //$('#popupAddBaseLine').modal('toggle');
                      $("#VeecBaselineUpgradeGridView").html(response.baselinePartialView);
                    
                  };
                  //$("#loading-image").hide();
              }
          });
   
    
}
function clearBaselineValues() {
    $('#BaselineEquipmentId').val('');
    $('#AddBaselineEquipment')[0].reset();
}

function hideShowProduct() {
    var lamptype = $("#LampType").val();
    var UpgradeAssetLifetimeReferencetype = $("#UpgradeAssetLifetimeReference").val();
    if ((lamptype == 1 || lamptype == 2) && (UpgradeAssetLifetimeReferencetype == 1 || UpgradeAssetLifetimeReferencetype == 4)) {
        $(".product").show();
        $("#ProductBrand").rules("add", {
            required: true,
        });
        $("#ProductBrandLabel").addClass("required")
        $("#ProductModel").rules("add", {
            required: true,
        });
        $("#ProductModelLabel").addClass("required")
    } else {
        $(".product").hide();
        $("#ProductBrand").rules("add", {
            required: false,
        });
        $("#ProductBrandLabel").removeClass("required")
        $("#ProductModel").rules("add", {
            required: false,
        });
        $("#ProductModelLabel").removeClass("required")
    }
}

function typeControllerValidate() {
    if ($("#UpgradeAssetLifetimeReference").val() == 2) {
        $("#FirstControllerLabel").addClass("required");
        $("#FirstController").rules("add", {
            required: true,
        });
    }
    else {
        $("#FirstControllerLabel").removeClass("required");
        $("#FirstController").rules("add", {
            required: false,
        });
    }
}
function showhideRatedHourOrLampPower() {
    
    var lamptype = $("#LampType").val();
    var UpgradeAssetLifetimeReferencetype = $("#UpgradeAssetLifetimeReference").val();
    if ((lamptype == 1 && UpgradeAssetLifetimeReferencetype == 2) || ((lamptype == 3) && (UpgradeAssetLifetimeReferencetype == 2 || UpgradeAssetLifetimeReferencetype == 4))) {
        $(".rlh").show();
        $(".nlp").show();
        $("#RatedLifetimeHours").rules("add", {
            required: true,
        });
        $("#RatedLifetimeHoursLabel").addClass("required")
        $("#NominalLampPower").rules("add", {
            required: true,
        });
        $("#NominalLampPowerLabel").addClass("required")
    } else {
        $(".rlh").hide();
        $(".nlp").hide();
        $("#RatedLifetimeHours").rules("add", {
            required: false,
        });
        $("#RatedLifetimeHoursLabel").removeClass("required")
        $("#NominalLampPower").rules("add", {
            required: false,
        });
        $("#NominalLampPowerLabel").removeClass("required")
    }
    if (lamptype == 3) {
        $(".nlp").show();
        $("#NominalLampPower").rules("add", {
            required: true,
        });
        $("#NominalLampPowerLabel").addClass("required")
    }
}
