$(function () {
    $(".glyphicon-off").click(function () {
        var $this = $(this);
        
        $.post({
            url: "/Index/ToggleVMState",
            data: {
                vmName: $.trim($(this).closest("tr").find("td:eq(0)").html()),
                state: $(this).hasClass("btn-danger") ? 2 : 3
            },
            success: function (data) {
                $this.toggleClass("btn-success btn-danger");
            }
        });
    });
});