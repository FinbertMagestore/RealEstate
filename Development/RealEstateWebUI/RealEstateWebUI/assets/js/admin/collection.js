// create collection
//------------------
var file;
function addRuleForCollectionCreate() {
    return $.ajax({
        type: 'GET',
        url: "/admin/collections/addRule?index=" + CollectionDetail.index,
        async: true,
        success: function (data) {
            $("#smartFilterCondition").append(data);
            CollectionDetail.index++;
        },
        error: function () {
        }
    });
}
function submitCollectionForm() {
    for (instance in CKEDITOR.instances)
        CKEDITOR.instances[instance].updateElement();
    var TblRules = [];
    var formData = new FormData();
    var collectionForm = $("#collectionForm");
    if (!collectionForm.valid()) return;
    var u = collectionForm.serializeArray();
    var listColumnName = [], listRelation = [], listConditionValue = [];
    for (var i = 0; i < u.length; i++) {
        if (u[i].name == "TblRules[].ColumnName") {
            listColumnName.push(u[i].value);
        } else if (u[i].name == "TblRules[].Relation") {
            listRelation.push(u[i].value);
        } else if (u[i].name == "TblRules[].ConditionValue") {
            listConditionValue.push(u[i].value);
        } else {
            formData.append(u[i].name, u[i].value);
        }
    }
    for (var i = 0; i < listColumnName.length; i++) {
        TblRules[i] = [];
        TblRules[i][0] = listColumnName[i];
        TblRules[i][1] = listRelation[i];
        TblRules[i][2] = listConditionValue[i];
    }
    for (var i = 0; i < listColumnName.length; i++) {
        var name = "TblRules[" + i + "].ColumnName";
        formData.append(name, TblRules[i][0]);
        name = "TblRules[" + i + "].Relation";
        formData.append(name, TblRules[i][1]);
        name = "TblRules[" + i + "].ConditionValue";
        formData.append(name, TblRules[i][2]);
    }
    if (file) {
        formData.append("file", file);
    }
    return $.ajax({
        url: "/admin/collections/create/",
        type: "POST",
        data: formData,
        processData: false,
        contentType: false,
        success: function (data) {
            if (!isNaN(data.trim())) {
                window.location.href = "/admin/collections/" + data;
            } else {
                //alert("Thêm sản phẩm lỗi");
                //window.location.href = "/admin/collections/create";
                document.getElementById("strMessage").innerHTML = "<div class=\"alert alert-danger\"><button class=\"close\" data-dismiss=\"alert\">×</button>" + data + "</div>";
            }
        }

    });
}

function removeRule(index) {
    $("#rules_" + index).remove();
}

function UploadFile() {
    //view image at base64
    var files = document.getElementById("ImageCollection").files;
    if (files && files[0]) {
        file = files[0];
        var FR = new FileReader();
        var srcData;
        FR.onload = function (e) {
            srcData = e.target.result;
            var temp = "<a data-target=\"#image\" data-toggle=\"modal\" data-id=\"" + srcData + "\" onclick=\"showImage('" + srcData + "')\"><img class=\"product-photo-item\" height=\"100\" width=\"100\" src=\"" + srcData + "\"/></div></a>";
            document.getElementById("imageAdd").innerHTML = temp;
        };
        FR.readAsDataURL(files[0]);
    }
}
function showImage(value) {
    //var myBookId = $(this).data('id');
    $(".modal-body #imageCollectionAdd").attr('src', value);
}

function openUploadImage() {
    $('#ImageCollection').click();
}
function deleteCollectionImage() {
    file = null;
    $('#image').modal('hide');
    $("#imageAdd").addClass("hide");
}

// detail collection
//-----------------
//var indexAddRule_CollectionDetail = CollectionDetail.index;
function addRuleForCollectionDetail() {
    return $.ajax({
        type: 'GET',
        url: "/admin/collections/addRule?index=" + CollectionDetail.index,
        async: true,
        success: function (data) {
            $("#smartFilterCondition").append(data);
            CollectionDetail.index++;
        },
        error: function () {
        }
    });
}
function removeRule(index) {
    $("#rules_" + index).remove();
}
function submitUpdateCollectionForm(collectionID) {
    for (instance in CKEDITOR.instances)
        CKEDITOR.instances[instance].updateElement();
    var TblRules = [];
    var formData = new FormData();
    var collectionForm = $("#collectionForm");
    if (!collectionForm.valid()) {
        return;
    }
    var u = collectionForm.serializeArray();
    var listColumnName = [], listRelation = [], listConditionValue = [], listRuleID = [];
    for (var i = 0; i < u.length; i++) {
        if (u[i].name == "TblRules[].ColumnName") {
            listColumnName.push(u[i].value);
        } else if (u[i].name == "TblRules[].Relation") {
            listRelation.push(u[i].value);
        } else if (u[i].name == "TblRules[].ConditionValue") {
            listConditionValue.push(u[i].value);
        } else {
            formData.append(u[i].name, u[i].value);
        }
    }
    for (var i = 0; i < listColumnName.length; i++) {
        TblRules[i] = [];
        TblRules[i][0] = listColumnName[i];
        TblRules[i][1] = listRelation[i];
        TblRules[i][2] = listConditionValue[i];
    }
    for (var i = 0; i < listColumnName.length; i++) {
        var name = "TblRules[" + i + "].ColumnName";
        formData.append(name, TblRules[i][0]);
        name = "TblRules[" + i + "].Relation";
        formData.append(name, TblRules[i][1]);
        name = "TblRules[" + i + "].ConditionValue";
        formData.append(name, TblRules[i][2]);
    }
    if (file) {
        formData.append("file", file);
    }
    return $.ajax({
        url: "/admin/collections/" + collectionID,
        type: "POST",
        data: formData,
        processData: false,
        contentType: false,
        success: function (data) {
            if (!isNaN(data)) {
                window.location.href = "/admin/collections/" + data + "?strSuccess=update1";
            } else {
                //alert("Thêm sản phẩm lỗi");
                //window.location.href = "/admin/collections/create";
                document.getElementById("strMessage").innerHTML = "<div class=\"alert alert-danger\"><button class=\"close\" data-dismiss=\"alert\">×</button>" + data + "</div>";
            }
        }

    });
}

function UploadFile() {
    $("#imageAdd").removeClass("hide");
    $("#oldImage").addClass("hide");
    //view image at base64
    var files = document.getElementById("ImageCollection").files;
    if (files && files[0]) {
        file = files[0];
        var FR = new FileReader();
        var srcData;
        FR.onload = function (e) {
            srcData = e.target.result;
            var temp = "<a data-target=\"#image\" data-toggle=\"modal\" data-id=\"" + srcData + "\" onclick=\"showImage('" + srcData + "')\"><img class=\"product-photo-item\" height=\"100\" width=\"100\" src=\"" + srcData + "\" alt=\"\"/></div></a>";
            document.getElementById("imageAdd").innerHTML = temp;
        };
        FR.readAsDataURL(files[0]);
    }
}
function showImage(value) {
    //var myBookId = $(this).data('id');
    $(".modal-body #imageCollectionAdd").attr('src', value);
}
function showImageExist() {
    var src = CollectionDetail.image;
    $(".modal-body #imageCollectionAdd").attr('src', src);
}

function submitDeleteCollectionForm(collectionID) {
    window.location.href = "/admin/collections/delete/" + collectionID;
}

function deleteCollectionImageExist(collectionID) {
    return $.ajax({
        type: 'GET',
        url: "/admin/collections/deleteImage?collectionID=" + CollectionDetail.id,
        async: true,
        success: function (data) {
            if (!isNaN(data)) {
                window.location.href = "/admin/collections/" + data + "?strSuccess=update1";
            }
            else {
                document.getElementById("strMessage").innerHTML = "<div class=\"alert alert-danger\"><button class=\"close\" data-dismiss=\"alert\">×</button>" + data + "</div>";
            }
        },
        error: function () {
        }
    });
}

function openUploadImage() {
    $('#ImageCollection').click();
}
function deleteCollectionImage() {
    $('#image').modal('hide');
    $("#imageAdd").addClass("hide");
    file = null;
}

//delete collection product
function removeProductOutCollection(collectionProductID, collectionID) {
    window.location.href = "/admin/collections/deleteProductOutCollection/" + collectionID + "?collectionProductID=" + collectionProductID;
}

function suggestCollection() {
    clearTimeout(delayTimer);
    var delayTimer = setTimeout(function () {
        var temp = $("#ConditionFindProduct").val();
        return $.ajax({
            type: 'GET',
            url: "/admin/collections/dropdown?productID=" + ProductDetail.productID + "&query=" + temp,
            async: true,
            success: function (data) {
                document.getElementById("collections").innerHTML = "";
                document.getElementById("collections").innerHTML = data;
                //alert(data);
                //$("#ConditionFindProduct").focus();
            },
            error: function () {
            }
        });
    }, 300);
}
// choice collection for product
function choiceCollection() {
    $("#collectionDropdown").submit();
}

// dropdown collections
function viewCollections(productID) {
    return $.ajax({
        type: 'GET',
        url: "/admin/collections/dropdown?productID=" + productID,
        async: true,
        success: function (data) {
            document.getElementById("collections").innerHTML = data;
        },
        error: function () {
        }
    });
}

function submitDeleteCollection() {
    $('#deleteCollection').modal('hide');
    window.location = "/admin/collections/delete/" + CollectionDetail.id;
}