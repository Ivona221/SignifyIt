

    /**
     * * Shows ui notifications about user interactions
     * note! for translation include 'notifications' resource translation file
     * @returns {} 
     */
    function Notifications() {
        var me = {};
        toastr.options = {
            "onclick": null,
            "showDuration": null,
            "timeOut": "0",
            "extendedTimeOut": "0",
            "tapToDismiss": false
        };
        var autoLocalize = false;
        var isLoadingModalInprogrss = false;
        
        /**
         * Shows notification message
         * @returns {} 
         */
        function show(type, title, content) {
            var title = title;
            if (typeof (content) == 'undefined' ||
                content == null) {
                content = "";
            }
            toastr.options = {
                "onclick": null,
                "showDuration": "5000",
                "timeOut": "5000",
                "extendedTimeOut": "1000",
                "tapToDismiss": false
            }
            // show 
            toastr[type](title, content);

            autoLocalize = false;
        }

        /**
         * @desc Show success notification message 
         * @param {string} title
         * @param {} content 
         */
        me.success = function (title, content) {
            var type = 'success';
            var icon = 'fa fa-check-square-o';
            show(type, title, content, icon);
        };

        /**
      * @desc Show error notification message 
      * @param {string} title
      * @param {} content 
      * @returns {} 
      */
        me.error = function (title, content) {
            var type = 'error';
            show(type, title, content);
        };
        
        /**
      * @desc Show warning notification message 
      * @param {string} title
      * @param {} content 
      * @returns {} 
      */
        me.warning = function (title, content) {
            var type = 'warning';
            show(type, title, content);
        };

        /**
     * @desc Show info notification message 
     * @param {string} title
     * @param {} content 
     * @returns {} 
     */
        me.info = function (title, content) {
            var type = 'info';
            show(type, title, content);
        };

        /**
         * Show loading notification in the top right corner
         * By default is will show 'Loading...' if you want to override add translation key in the param
         * @param {} msg 
         * @returns {} 
         */
        me.showLoading = function (msg) {
            //$("#overlay").css("display", "block");
            var message = msg || "Loading...";
            isLoadingModalInprogrss = true;
            var url = "/images/loading-notif.gif";
            var htmlMsg = '<img style="height:20px; width:20px"  src="' + url + '" />';

            toastr.options = {
                "onclick": null,
                "showDuration": null,
                "timeOut": "0",
                "extendedTimeOut": "0",
                "tapToDismiss": false
            };
            toastr.info(message, htmlMsg);
        };

        /**
         * Hide side notification message loading 
         * @returns {} 
         */
        me.hideLoading = function () {
            $("#overlay").css("display", "none");
            isLoadingModalInprogrss = false;
            toastr.clear();
        };       

        return me;
    }

