function changeColor_ViewProducts(nameClass) {
    $('.productsCreateDESC').removeAttr('style');
    $('.productsBestSelling').removeAttr('style');
    $('.productsDiscountPrice').removeAttr('style');

    $('.' + nameClass).attr('style', 'color: black')
}

function getNumberVariantInCart() {
    return $.ajax({
        type: 'GET',
        url: "/client/cart/GetNumberVariantInCart",
        async: true,
        success: function (data) {
            var numberVariant = parseInt(data);
            if (!Number.isInteger(numberVariant)) {
                numberVariant = 0;
            }
            $('.numberCart').text(numberVariant);
        },
        error: function () {
            $('.numberCart').text(0);
        }
    });
}


function buyProduct(variantID, numberVariant) {
    if (numberVariant == 0) {
        var temp = parseInt($("#orderNumberVariant").val());
        if (temp > 0) {
            numberVariant = temp;
        }
    }
    if (numberVariant > 0) {
        return $.ajax({
            type: 'GET',
            url: "/client/cart/addVariantToCart?variantID=" + variantID + "&numberVariant=" + numberVariant,
            async: true,
            success: function (data) {
                if (typeof data == "undefined") {
                    data = 0;
                }
                $('.numberCart').text(data);
            },
            error: function () {
            }
        });
    }
}

function showModalNewLetter() {
    $("#bizweb-modal").empty();
    $("#bizweb-modal").load('/client/home/newLetterPopup');
}

function changeSortListProduct(typeView) {
    var sortValue = $('#sortListProduct_Grid').val();
    var numberView = $('#numberView').val();
    window.location.href = "/client/products?view=" + typeView + "&numberView=" + numberView + "&sortOrder=" + sortValue;
}

function removeVariantInCart(cartItemID) {
    window.location.href = "/client/cart/removeCartItem/" + cartItemID;
}

function viewOrderProducts(view) {
    if (view == "grid") {
        $(".styleViewIsGrid").removeClass("text-muted");
    } else {
        $(".styleViewIsList").removeClass("text-muted");
    }
}

function subOrderNumberVariant() {
    var orderNumberVariant = parseInt($("#orderNumberVariant").val());
    if (orderNumberVariant > 1) {
        orderNumberVariant--;
    }
    $("#orderNumberVariant").val(orderNumberVariant.toString());
}
function addOrderNumberVariant() {
    var orderNumberVariant = parseInt($("#orderNumberVariant").val());
    orderNumberVariant++;
    $("#orderNumberVariant").val(orderNumberVariant.toString());
}

function billingProvinceChange(billingProvinceID) {
    $("#BillingDistrictID").html("");
    var procemessage = "<option value='0'> --- Chọn quận huyện ---</option>";
    $("#BillingDistrictID").html(procemessage).show();
    var url = "/client/checkout/getDistrictsByProvinceID/" + billingProvinceID;

    $.ajax({
        url: url,
        cache: false,
        type: "POST",
        success: function (data) {
            if (data != null) {
                var markup = "<option value='0'>--- Chọn quận huyện ---</option>";
                for (var i = 0; i < data.length; i++) {
                    markup += "<option value=" + data[i].DistrictID + ">" + data[i].DistrictName + "</option>";
                }
                $("#BillingDistrictID").html(markup).show();
            }
        },
        error: function (reponse) {
            alert("error : " + reponse);
        }
    });
}

function shippingProvinceChange(billingProvinceID) {
    $("#ShippingDistrictID").html("");
    var procemessage = "<option value='0'> --- Chọn quận huyện ---</option>";
    $("#ShippingDistrictID").html(procemessage).show();
    var url = "/client/checkout/getDistrictsByProvinceID/" + billingProvinceID;

    $.ajax({
        url: url,
        cache: false,
        type: "POST",
        success: function (data) {
            if (data != null) {
                var markup = "<option value='0'>--- Chọn quận huyện ---</option>";
                for (var i = 0; i < data.length; i++) {
                    markup += "<option value=" + data[i].DistrictID + ">" + data[i].DistrictName + "</option>";
                }
                $("#ShippingDistrictID").html(markup).show();
            }
        },
        error: function (reponse) {
            alert("error : " + reponse);
        }
    });

}