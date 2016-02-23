//function checkFacebookLoginState() {
//        FB.getLoginStatus(function (response) {
//            facebookStatusCallback(response);
//        });
//    }

//function facebookStatusCallback(response) {
//    console.log('facebookStatusCallback');
//    console.log(response);
//    // The response object is returned with a status field that lets the
//    // app know the current login status of the person.
//    // Full docs on the response object can be found in the documentation
//    // for FB.getLoginStatus().
//    if (response.status === 'connected') {
//        console.log("logged facebook")
//    } else if (response.status === 'not_authorized') {
//        console.log("not autho");
//    } else {
//        console.log("else_fb");
//        FB.login(function (response) {
//            // Handle the response object, like in statusChangeCallback() in our demo
//            // code.
//        });
//    }
//}

function delogIfLogged(completionHandler) {
            FB.getLoginStatus(function (response) {
                if (response.status === 'connected') {
                    FB.logout(function(response) {
                        completionHandler();
                    });
                } else {
                    completionHandler();
                }
            });
}

function doLoginOrRegister() {
    delogIfLogged(function() {
        FB.login(function (responseLogin) {
            if (responseLogin.status === 'connected') {
                var userId = responseLogin.authResponse.userID;
                var accessToken = responseLogin.authResponse.accessToken;

                //Récupération des informations de l'utilisateur
                FB.api('/me?fields=id,name,email,last_name,first_name', function (responseMe) {
                    if (responseMe.email == undefined || responseMe.email == '') {
                        alert("Vous n'avez pas reseigné d'adresse email sur votre compte Facebook");
                        return;
                    }

                    var email = responseMe.email;
                    var lastName = responseMe.last_name;
                    var firstName = responseMe.first_name;

                    //Envoi du formulaire
                    var formParams = {
                        Email: email,
                        ProviderType: "Facebook",
                        FirstName: firstName,
                        LastName: lastName,
                        ProviderUserId: userId
                    }
                    sendExternalLoginForm(formParams);
                });

            } else if (responseLogin.status === 'not_authorized') {
                console.log("Not authorized to log in with Facebook");
            } else {
                console.log("Can't log in with Facebook, see full response below to debug : ");
                console.log(responseLogin);
            }
        }, { scope: 'email' });
    });
}

function sendExternalLoginForm(formParams) {
    var form = document.createElement("form");
    form.setAttribute("method", "post");
    form.setAttribute("action", "LoginOrRegisterExternal");

    for (var key in formParams) {
        if (formParams.hasOwnProperty(key)) {
            var hiddenField = document.createElement("input");
            hiddenField.setAttribute("type", "hidden");
            hiddenField.setAttribute("name", key);
            hiddenField.setAttribute("value", formParams[key]);

            form.appendChild(hiddenField);
        }
    }
    document.body.appendChild(form);
    form.submit();
}

//Facebook SDK Init
window.fbAsyncInit = function() {
    FB.init({
        appId: '1703651573245872',
        cookie: true, // enable cookies to allow the server to access 
        // the session
        xfbml: true, // parse social plugins on this page
        version: 'v2.5' // use graph api version 2.5
    });
};

// Load the SDK asynchronously
(function (d, s, id) {
    var js, fjs = d.getElementsByTagName(s)[0];
    if (d.getElementById(id)) { return; }
    js = d.createElement(s); js.id = id;
    js.src = "//connect.facebook.net/en_US/sdk.js";
    fjs.parentNode.insertBefore(js, fjs);
}(document, 'script', 'facebook-jssdk'));
