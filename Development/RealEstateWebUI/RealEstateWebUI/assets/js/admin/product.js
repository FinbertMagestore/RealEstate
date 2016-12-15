// create product
//---------------
// click to display short description
function showShortDescription() {
    $("#showShortDescription").addClass("hide");
    $("#shortDescription").removeClass("hide");
}
// read file selected and display image add format base 64
var MultipleImage = [];
var numImageAdd = 0;
function UploadFileCreateProduct() {
    //view image at base64
    $("#noImageDisplay").addClass("hide");
    var files = document.getElementById("MultipleImage").files;
    for (var i = 0; i < files.length; i++) {
        if (files && files[i]) {
            MultipleImage.push(files[i]);
            var FR = new FileReader();
            var srcData;
            FR.onload = function (e) {
                srcData = e.target.result;

                document.getElementById("imageAdd").innerHTML += "<div id=\"imageAdd" + numImageAdd + "\" class=\"col-sm-3\">" +
                                    "<img class=\"product-photo-item\" height=\"170\" width=\"170\" src=\"" + srcData + "\"/>" +
                                    "<div><a id=\"deleteItem\" data-target=\"#deleteImage\" data-toggle=\"modal\" onclick=\"showDataModalDeleteImageAdd_Create(" + numImageAdd + ")\">Xóa ảnh</a></div>" +
                                "</div>";

                $("#imageAdd").removeClass("hide");
                numImageAdd++;
            };
            FR.readAsDataURL(files[i]);
        }
    }
}
function openUploadImageCreateProduct() {
    $('#MultipleImage').click();
}

function submitSaveCreateProduct() {
    var formData = new FormData();
    for (instance in CKEDITOR.instances)
        CKEDITOR.instances[instance].updateElement();
    var productForm = $("#productForm");
    if (!productForm.valid()) return;
    var u = productForm.serializeArray(), r;
    for (r = 0; r < u.length; ++r) {
        if (u[r].name == "MultipleImage") break;
        else formData.append(u[r].name, u[r].value);
    }
    for (var i = 0; i < MultipleImage.length; i++) {
        var s = MultipleImage[i];
        formData.append("UploadeImages", s);
    }
    return $.ajax({
        url: "/admin/products/create",
        type: "POST",
        data: formData,
        processData: false,
        contentType: false,
        success: function (data) {
            if (!isNaN(data)) {
                window.location.href = "/admin/products/" + data;
            } else {
                document.getElementById("strMessage").innerHTML = "<div class=\"alert alert-danger\"><button class=\"close\" data-dismiss=\"alert\">×</button>" + data + "</div>";
            }
        }

    });
}

//delete image new add at format base 64
var imageAddDelete;
function showDataModalDeleteImageAdd_Create(imageAddID) {
    imageAddDelete = imageAddID;
}

// accept delete image
function submitDeleteImageAddProduct() {
    $('#deleteImage').modal('hide');

    // delete image when view not submit
    var elementDelete = "#imageAdd" + imageAddDelete;
    $(elementDelete).remove();
    MultipleImage.splice(imageAddDelete, 1);
}

function changeWeightUnit(unit) {
    $("input[name='Variant.WeightUnit']").val(unit);
    $("#weight-unit").html(unit);
}

function toggleAutoGenerate() {
    if ($("#AutoGenerate").val() == "true") {
        $("#AutoGenerate").val('false');
        $("#variants-option").addClass("hide");
        $("#btnCreateOption").removeClass("hide");
        $("#btnDisposeOption").addClass("hide");
    } else {
        $("#AutoGenerate").val('true');
        $("#variants-option").removeClass("hide");
        $("#btnCreateOption").addClass("hide");
        $("#btnDisposeOption").removeClass("hide");
    }
}

var options_CreateProduct = [
    {
        name: "",
        value: []
    }
];
// option
function addNewOption_CreateProduct() {
    options_CreateProduct = getOptions(options_CreateProduct.length);
    options_CreateProduct.push({ name: "", value: [] })
    reloadOptionElements(options_CreateProduct);
    changeListVariant();
    if (options.length > 2) {
        $(".addNewOption").addClass("hide");
    }
}

function removeOption_CreateProduct(optionIndex) {
    //if (optionIndex > 0) {
    options_CreateProduct = getOptions(options_CreateProduct.length);
    options_CreateProduct.splice((optionIndex - 1), 1);
    reloadOptionElements(options_CreateProduct);
    if (options_CreateProduct.length < 3) {
        $(".addNewOption").removeClass("hide");
    }
    //}
}

function getOptions(length) {
    var options = [];
    for (var i = 1; i <= length; i++) {
        var optionNameClass = ".option" + i + " .optionName";
        var optionValueClass = ".option" + i + " .taginput";
        var optionElement = {
            name: $(optionNameClass).val(),
            value: $(optionValueClass).val()
        };
        options.push(optionElement);
    }
    return options;
}

function reloadOptionElements(options) {
    var result = "";
    var templateHtml = "<tr class=\"options option{index}\"><td><input class=\"form-control optionName\" type=\"text\" value='{name}' name=\"Options[{optionIndex}].OptionName\"></td>";
    templateHtml += "<td><input class=\"taginput\" name=\"Options[{optionIndex}].OptionValue\" type=\"text\" data-role=\"tagsinput\" value='{value}' style=\"display: none;\" onchange=\"changeListVariant()\"><td/>";
    templateHtml += "<td><a class='removeOption{index}' onclick=\"removeOption_CreateProduct({index})\"><i class=\"fa fa-trash-o fa-2x\"></i></a></td></tr>";
    for (var i = 1; i <= options.length; i++) {
        result += templateHtml.replace(/{optionIndex}/g, (i - 1)).replace(/{index}/g, i).replace(/{name}/g, options[(i - 1)].name).replace(/{value}/g, options[(i - 1)].value);
    }
    $("#options tbody").html(result);
    changeListVariant();
    $('input.taginput').tagsinput('refresh');
    if (options.length == 1) {
        $(".removeOption1").addClass("hide");
    }
}

function changeListVariant() {
    $(".list_variant tbody").html("");
    var variants = "";
    var option1 = $(".option1 .taginput").val(), option2 = $(".option2 .taginput").val(), option3 = $(".option3 .taginput").val();
    var option1Array = option2Array = option3Array = [];
    if (options_CreateProduct.length == 1) {
        option1Array = option1.split(",");
    } else if (options_CreateProduct.length == 2) {
        option1Array = option1.split(",");
        option2Array = option2.split(",");
    } else if (options_CreateProduct.length == 3) {
        option1Array = option1.split(",");
        option2Array = option2.split(",");
        option3Array = option3.split(",");
    }
    variants = creatHtmlVariant(option1Array, option2Array, option3Array);
    if (variants && variants != "") {
        $(".list_variant").removeClass("hide");
        $(".list_variant tbody").html(variants);
    } else {
        $(".list_variant").addClass("hide");
    }
}

function creatHtmlVariant(option1Array, option2Array, option3Array) {
    var result = "";
    var templateHtmlBefore = "<tr><td>" +
                    "<input class='checkbox is-create' type='checkbox' name='Variants[{index}].IsCreate' checked='checked' value='true'/>" +
                "</td>";
    var templateHtmlAfter = "<td><input class='form-control' type='number' name='Variants[{index}].VariantPrice' value='0'></td>";
    templateHtmlAfter += "<td><input class='form-control' type='text' name='Variants[{index}].VariantSKU' value=''></td>";
    templateHtmlAfter += "<td><input class='form-control' type='text' name='Variants[{index}].VariantBarcode' value=''></td></tr>";

    var variants = [];
    if (option1Array && option1Array.length > 0) {
        for (var i = 0; i < option1Array.length; i++) {
            if (option1Array[i] && option1Array[i] != "") {
                var variant = "";
                var variant1 = "<td>" +
                        "<span class='option-value1'>{value1}</span>" +
                        "<input type='hidden' name='Variants[{index}].Option1' value='{value1}'>";
                variant1 = variant1.replace(/{value1}/g, option1Array[i]);
                if (option2Array && option2Array.length > 0) {
                    for (var j = 0; j < option2Array.length; j++) {
                        if (option2Array[j] && option2Array[j] != "") {
                            var variant2 = variant1 + " * ";
                            variant2 += "<span class='option-value2'>{value2}</span>" +
                                "<input type='hidden' name='Variants[{index}].Option2' value='{value2}'>";
                            variant2 = variant2.replace(/{value2}/g, option2Array[j]);

                            if (option3Array && option3Array.length > 0) {
                                for (var k = 0; k < option3Array.length; k++) {
                                    if (option3Array[k] && option3Array[k] != "") {
                                        var variant3 = variant2 + " * ";
                                        variant3 += "<span class='option-value3'>{value3}</span>" +
                                            "<input type='hidden' name='Variants[{index}].Option3' value='{value3}'>";
                                        variant3 = variant3.replace(/{value3}/g, option3Array[k]);

                                        variant3 += "</td>";
                                        variants.push(variant3);
                                    } else {
                                        variant2 += "</td>";
                                        variants.push(variant2);
                                    }
                                }
                            } else {
                                variant2 += "</td>";
                                variants.push(variant2);
                            }

                        } else {
                            variant1 += "</td>";
                            variants.push(variant1);
                            variant = "";
                        }
                    }
                } else {
                    variant1 += "</td>";
                    variants.push(variant1);
                    variant = "";
                }

            }
        }
        var numVariant = 0;
        if (options_CreateProduct.length == 1) {
            numVariant = option1Array.length;
        } else if (options_CreateProduct.length == 2) {
            numVariant = option1Array.length * option2Array.length
        } else if (options_CreateProduct.length == 3) {
            numVariant = option1Array.length * option2Array.length * option3Array.length;
        }
        for (var i = 0; i < numVariant; i++) {
            if (variants[i] && variants[i] != "") {
                result += templateHtmlBefore.replace(/{index}/g, i) + variants[i].replace(/{index}/g, i) + templateHtmlAfter.replace(/{index}/g, i);
            }
        }
        return result;
    } else {
        return "";
    }

}

function addTag(tag) {
    var tagNew = $("input[name=Tags]").val();
    if (!tagNew.endsWith(',')) {
        tagNew += ",";
    }
    if (tagNew.startsWith(',')) {
        tagNew = tagNew.substring(1, tagNew.length);
    }
    tagNew += tag.innerHTML;
    $("input[name=Tags]").val(tagNew);
    $('input[name=Tags]').tagsinput('add', tag.innerHTML);
}
String.prototype.endsWith = function (suffix) {
    return this.indexOf(suffix, this.length - suffix.length) !== -1;
};
String.prototype.startsWith = function (needle) {
    return (this.indexOf(needle) == 0);
};

// detail product
//---------------
// click to display short description
function showShortDescription() {
    $("#showShortDescription").addClass("hide");
    $("#shortDescription").removeClass("hide");
}
// read file selected and display image add format base 64
function UploadFileProductDetail(productID) {
    //document.forms['uploadImage'].submit();
    var formData = new FormData();
    var files = document.getElementById("fileProductDetail").files;
    if (files && files[0]) {
        formData.append("fileProductDetail", files[0]);
    }
    formData.append("id", productID);
    return $.ajax({
        type: 'POST',
        url: "/admin/image/uploadImageProduct/" + productID,
        data: formData,
        processData: false,
        contentType: false,
        success: function (data) {
            window.location.href = "/admin/products/" + productID + "?messageUpload=" + data;
        },
        error: function () {
        }
    });
}
function openUploadImageProductDetail() {
    $('#fileProductDetail').click();
}

function submitSaveProductDetail() {
    document.forms['productForm'].submit();
}

// pass data for modal confirm delete image
function showDataModal(imageIDValue, productIDValue) {
    ProductDetail.imageIDDelete = imageIDValue;
    ProductDetail.productID = productIDValue;
}

//function showDataModalDeleteImageAdd_Detail(imageAddID) {
//    ProductDetail.imageAddDelete = imageAddID;
//    ProductDetail.imageIDDelete = 0;
//}

// accept delete image
function submitDeleteImageDetailProduct() {
    $('#deleteImage').modal('hide');
    window.location = "/admin/image/delete/" + ProductDetail.imageIDDelete + "?productID=" + ProductDetail.productID;
}
// delete product
function showModalDeleteProduct(productIDValue) {
    ProductDetail.productID = productIDValue;
}
function submitDeleteProduct() {
    $('#deleteProduct').modal('hide');
    window.location = "/admin/products/delete/" + ProductDetail.productID;
}

//add image from url
function submitAddImageFromUrl(productID) {
    $('#addImageFromUrl').modal('hide');
    var imageUrl = $("#src")[0].value;
    window.location = "/admin/image/addImageFromUrl" + "?productID=" + productID + "&url=" + imageUrl;
}

// dropdown products
function viewProducts() {
    return $.ajax({
        type: 'GET',
        url: "/admin/products/dropdown?collectionID=" + CollectionDetail.id,
        async: true,
        success: function (data) {
            document.getElementById("products").innerHTML = data;
        },
        error: function () {
        }
    });
}

function suggestProduct() {
    clearTimeout(delayTimer);
    var delayTimer = setTimeout(function () {
        var temp = $("#ConditionFindProduct").val();
        return $.ajax({
            type: 'GET',
            url: "/admin/products/dropdown?collectionID=" + CollectionDetail.id + "&query=" + temp,
            async: true,
            success: function (data) {
                document.getElementById("products").innerHTML = "";
                document.getElementById("products").innerHTML = data;
                //alert(data);
                //$("#ConditionFindProduct").focus();
            },
            error: function () {
            }
        });
    }, 300);
}
// choice add product to collection
function choiceProduct() {
    $("#productDropdown").submit();
}

function deleteVariant(variantID, productID) {
    return $.ajax({
        type: 'GET',
        url: "/admin/products/dropdown?collectionID=" + CollectionDetail.id + "&query=" + temp,
        async: true,
        success: function (data) {
            document.getElementById("products").innerHTML = "";
            document.getElementById("products").innerHTML = data;
            //alert(data);
        },
        error: function () {
        }
    });
}
//delete variant
function showDataModalDeleteVariant(variantIDValue, productIDValue) {
    ProductDetail.variantID = variantIDValue;
    ProductDetail.productID = productIDValue;
}

function submitDeleteVariant() {
    window.location = "/admin/products/" + ProductDetail.productID + "/variants/delete/" + ProductDetail.variantID;
}

function showViewOpionsModal(productIDValue) {
    return $.ajax({
        type: 'GET',
        url: "/admin/products/updateOptionsOfProduct/" + productIDValue,
        async: true,
        success: function (data) {
            document.getElementById("updateOptions").innerHTML = "";
            document.getElementById("updateOptions").innerHTML = data;
        },
        error: function () {
        }
    });
}

function removeCollectionOutProduct(collectionProductID, productID) {
    window.location.href = "/admin/products/deleteProductOutCollection/" + productID + "?collectionProductID=" + collectionProductID;
}
