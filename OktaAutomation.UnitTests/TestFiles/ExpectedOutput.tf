{
  "Type": "okta_app_oauth",
  "Name": "Dev-NavisphereVision_Tracking_API_SwaggerUI",
  "Attributes": {
    "label": "Dev-Vision.Tracking.API.SwaggerUI",
    "type": "browser",
    "grant_types": [
      "authorization_code",
      "implicit"
    ],
    "redirect_uris": [
      "https://localhost:5001/openapi/oauth2-redirect.html",
      "https://localhost:5000/openapi/oauth2-redirect.html",
      "http://localhost:5000/openapi/oauth2-redirect.html",
      "https://dev-vision.api.chrazure.cloud/tracking/openapi/oauth2-redirect.html"
    ],
    "response_types": [
      "token",
      "id_token",
      "code"
    ],
    "omit_secret": "true",
    "token_endpoint_auth_method": "none",
    "consent_method": "TRUSTED",
    "skip_groups": "true",
    "skip_users": "true",
    "ignore_changes": [
      "users",
      "groups"
    ]
  },
  "Lifecycle": {
    "ignore_changes": [
      "users",
      "groups"
    ]
  }
}