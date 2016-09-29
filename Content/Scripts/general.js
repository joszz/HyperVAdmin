$(function () {
    $("body").on("click", ".glyphicon-off", function () {
        var $this = $(this);

        if (confirm("Are you sure you want to toggle this VM " + ($(this).hasClass("btn-danger") ? "on" : "off") + "?")) {
            $.post({
                url: "Index/ToggleVMState",
                data: {
                    vmName: $.trim($(this).closest("tr").find("td:eq(0)").html()),
                    state: $(this).hasClass("btn-danger") ? 2 : 3
                },
                success: function (data) {
                    $this.toggleClass("btn-success btn-danger");
                }
            });
        }
    });

    $(".panel-heading button").click(function () {
        $(".panel").isLoading({
            text: "Loading",
            position: "overlay"
        });

        $.getJSON({
            url: "Index/GetVMs",
            type: "POST",
            success: function (data) {
                $("tbody tr:not(.hidden)").remove();

                $.each(data, function (index, value) {
                    var clone = $("tr.hidden").clone();

                    clone.find("td:eq(0)").html(value.Name);
                    clone.find("td:eq(1)").html(value.Description);
                    clone.find("td:eq(2)").html(value.CoresAmount);
                    clone.find("td:eq(3)").html(value.MemoryTotal + " " + value.MemoryAllocationUnits);
                    clone.find("td:eq(4)").html(value.MAC);
                    clone.find("button").addClass("btn-" + (value.State == 2 ? "success" : "danger"));
                    clone.removeClass("hidden cloneable");

                    clone.appendTo($("tbody"));
                });
            },
            complete: function () {
                $(".panel").isLoading("hide");
            }
        });
    });
});