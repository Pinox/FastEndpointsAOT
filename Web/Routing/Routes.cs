namespace Web.Routing;

public static class AppRoutes
{
    // fastendpoints/* (dev/test routes)
    public const string fastendpoints_prefix = "/fe3";
    public const string fastendpoints_getapikey = fastendpoints_prefix + "/getapikey";
    public const string fastendpoints_apikey = fastendpoints_prefix + "/apikey";
    public const string fastendpoints_jwt = fastendpoints_prefix + "/jwt";
    public const string fastendpoints_checkjwt = fastendpoints_prefix + "/checkjwt";
    public const string fastendpoints_cookie_login = fastendpoints_prefix + "/cookie/login";
    public const string fastendpoints_cookie = fastendpoints_prefix + "/cookie";

    // diagnostics
    public const string security_whoami = "/security/whoami";

    // claims verification
    public const string security_claims_verify = "/security/claims/verify";

    // policies
    public const string policies_AdminPolicyVerify = "/policies/AdminPolicyVerify";
    public const string policies_UserPolicyVerify = "/policies/UserPolicyVerify";
    public const string policies_user = "/policies/user";

    // admin
    public const string admin_login = "/admin/login";

    // customer
    public const string customer_login = "/customer/login";
    public const string customer_update = "/customer/update";
    public const string customer_save = "/customer/save";
    public const string customer_save2 = "/customer/save2";
    public const string customer_update_with_header = "/customer/update-with-header";
    public const string customer_list_recent = "/customer/list/recent";

    // inventory
    public const string inventory_manage_update = "/inventory/manage/update";
    public const string inventory_manage_create = "/inventory/manage/create";
    public const string inventory_get_product = "/inventory/get-product/{ProductID}";
    public const string inventory_list_recent = "/inventory/list/recent/{CategoryID}";

    // uploads
    public const string uploads_image_save = "/uploads/image/save";
    public const string uploads_image_save_typed = "/uploads/image/save-typed";
    public const string samples_binding_multipart_upload = "/samples/binding/multipart-upload";

    // tokens
    public const string tokens_login = "/tokens/login";

    // swagger release versioning samples
    public const string release_versioning_endpoint_a = "/release-versioning/endpoint-a";
    public const string release_versioning_endpoint_b = "/release-versioning/endpoint-b";

    // security scopes tests
    public const string testcases_scope_tests_any_scope_pass = "/test-cases/scope-tests/any-scope-pass";
    public const string testcases_scope_tests_any_scope_fail = "/test-cases/scope-tests/any-scope-fail";
    public const string testcases_scope_tests_all_scope_pass = "/test-cases/scope-tests/all-scope-pass";
    public const string testcases_scope_tests_all_scope_fail = "/test-cases/scope-tests/all-scope-fail";

    // global throttle error response test
    public const string testcases_global_throttle_error_response = "/test-cases/global-throttle-error-response";

    // validation testcases
    public const string testcases_object_array_validation_error_test = "/test-cases/object-array-validation-error-test";
    public const string testcases_list_validation_error_test = "/test-cases/list-validation-error-test";
    public const string testcases_list_in_list_validation_error_test = "/test-cases/list-in-list-validation-error-test";
    public const string testcases_dictionary_validation_error_test = "/test-cases/dictionary-validation-error-test";
    public const string testcases_array_validation_error_test = "/test-cases/array-validation-error-test";
    public const string testcases_pre_processor_is_run_on_validation_failure = "/test-cases/pre-processor-is-run-on-validation-failure";
    public const string testcases_pre_processor_shortcircuit_while_validator_fails = "/testcases/pre-processor-shortcircuit-while-validator-fails";
    public const string testcases_data_annotation = "/test-cases/data-annotation";

    // binding and other test routes
    public const string testcases_on_before_on_after_validate = "/test-cases/on-before-on-after-validate";
    public const string test_hydrated_test_url_generator_test = "/test/hydrated-test-url-generator-test/{id}/{guid:guid}/{stringBindFrom}/{nullableString}/{fromClaim}/{fromHeader}/{hasPermission}";
    public const string testcases_route_binding_test = "/test-cases/route-binding-test/{string}/{bool}/{int}/{long}/{double}/{decimal}";
    public const string testcases_query_arrays_of_objects_binding_test = "/test-cases/query-arrays-of-objects-binding-test";
    public const string testcases_query_object_binding_test = "/test-cases/query-object-binding-test";

    // security missing header/claim tests
    public const string testcases_missing_header_test = "/test-cases/missing-header-test";
    public const string testcases_missing_header_test_dont_throw = "/test-cases/missing-header-test/dont-throw";
    public const string testcases_missing_claim_test = "/test-cases/missing-claim-test";
    public const string testcases_missing_claim_test_dont_throw = "/test-cases/missing-claim-test/dont-throw";

    // tests & utilities
    public const string tests_iauth_injection = "/tests/iauth-injection";

    // routing tests
    public const string testcases_routing_offer = "/test-cases/routing/offer/{OfferId?}";
    public const string testcases_routing_user = "/test-cases/routing/user/{UserId}";

    // processors tests
    public const string testcases_processor_state_sharing = "/test-cases/processor-state-sharing";

    // processors post-processor tests
    public const string testcases_post_processor_handles_exception = "/testcases/post-processor-handles-exception";
    public const string testcases_post_processor_handles_exception_no_post_processor = "/testcases/post-processor-handles-exception/no-post-processor";
    public const string testcases_global_generic_processors = "/testcases/global-generic-processors";

    // misc tests
    public const string testcases_unit_test_concurrency = "/test-cases/unit-test-concurrency";
    public const string testcases_stj_infinite_recursion = "/testcases/stj-infinite-recursion";
    public const string testcases_mapper_test = "/test-cases/mapper-test";
    public const string testcases_global_prefix_override = "/test-cases/global-prefix-override/{Id}";
    public const string testcases_range = "/test-cases/range";

    // messaging tests
    public const string testcases_event_bus_test = "/test-cases/event-bus-test";
    public const string tests_generic_command_handler = "/tests/generic-command-handler";
    public const string tests_generic_command_handler_without_result = "/tests/generic-command-handler-without-result";
    public const string tests_command_handler = "/tests/command-handler";
    public const string tests_commands = "/tests/commands";

    // endpoints tests
    public const string testcases_plaintext = "/test-cases/plaintext/{Id}";

    // endpoints tests additional
    public const string testcases_included_validator = "/test-cases/included-validator";
    public const string testcases_query_param_creation_from_test_helpers = "/test-cases/query-param-creation-from-test-helpers/{ComplexId}/{ComplexIdString}";
    public const string testcases_event_stream = "/test-cases/event-stream";
    public const string testcases_number = "/test-cases/{number}";

    // basics
    public const string api_ping = "/api-ping";
    public const string basics_ping = "/ping";

    // endpoints tests (additional)
    public const string testcases_empty_request_test = "/test-cases/empty-request-test";
    public const string testcases_response_cache_bypass_test = "/test-cases/response-cache-bypass-test";
    public const string testcases_output_cache_bypass_test = "/test-cases/output-cache-bypass-test";

    // dependency injection tests
    public const string testcases_service_reg_gen_test = "/test-cases/service-reg-gen-test";
    public const string testcases_keyed_services_test = "/test-cases/keyed-services-test";

    // binding tests (ep without req)
    public const string testcases_ep_without_req_route_binding_test = "/test-cases/ep-witout-req-route-binding-test/{CustomerID:int}/{OtherID}";
    public const string testcases_ep_without_req_query_param_binding_test = "/test-cases/ep-witout-req-query-param-binding-test";

    // binding tests (collections)
    public const string testcases_json_array_binding_for_ienumerable_props = "/test-cases/json-array-binding-for-ienumerable-props";
    public const string testcases_dupe_param_binding_for_ienumerable_props = "/test-cases/dupe-param-binding-for-ienumerable-props";

    // samples
    public const string samples_versioning_item = "/samples/versioning/item/{id}";
    public const string samples_binding_items = "/samples/binding/items/{id}";

    // docs-only sample
    public const string doc_only_ver3 = "OnlyVer3";
}
