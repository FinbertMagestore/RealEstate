function submitDeleteVariant() {
    window.location = "/admin/products/" + VariantDetail.productID + "/variants/delete/" + VariantDetail.variantID;
}

function changeWeightUnit(unit) {
    $("input#WeightUnit").val(unit);
    $("#weight-unit").html(unit);
}

function uploadImageVariant() {
    $("#imageVariant").click();
}

function UploadFileVariantDetail(variantID, productID) {
    var formData = new FormData();
    var files = document.getElementById("imageVariant").files;
    if (files && files[0]) {
        formData.append("imageVariant", files[0]);
    }
    formData.append("productID", productID);
    return $.ajax({
        type: 'POST',
        url: "/admin/image/uploadImageVariant/" + variantID,
        data: formData,
        processData: false,
        contentType: false,
        success: function (data) {
            window.location.href = "/admin/products/" + productID + "/variants/" + variantID + "?strMessage=" + data;
        },
        error: function () {
        }
    });
}
function viewImageCreateVariant() {
    //view image at base64
    $(".next-media__blank-slate__content").addClass("hide");
    var files = document.getElementById("imageVariant").files;
    if (files && files[0]) {
        var FR = new FileReader();
        var srcData;
        FR.onload = function (e) {
            srcData = e.target.result;
            $(".imageVariantCreate").attr("src", srcData);

            $("#imageAdd").removeClass("hide");
        };
        FR.readAsDataURL(files[0]);
    }
}