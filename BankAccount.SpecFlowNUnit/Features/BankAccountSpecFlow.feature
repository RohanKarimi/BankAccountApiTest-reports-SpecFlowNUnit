Feature: SpecFlowFeature
	In order to verify bank account validation functionality
	As a tester who wants this job

@mytag
Scenario: Successful api call
	Given a request with url of 'https://api-test.afterpay.dev/'
	When valid request is posted to api with token 'Q7DaxRnFls6IpwSW1SQ2FaTFOf7UdReAFNoKY68L' and bankAccount 'GB09HAOE91311808002317' and content type of 'application/json' and url of 'api/v3/validate/bank-account'
	Then Api returns ok

@mytag
	Scenario: Unsuccessful api call with invalid token
	Given a request with url of 'https://api-test.afterpay.dev/'
	When invalid request is posted to api with invalid token '123456nFls6IpwSW1SQ2FaTFOf7UdReAFNoKY68L' and bankAccount 'GB09HAOE91311808002317' and content type of 'application/json' and url of 'api/v3/validate/bank-account'
	Then Api returns error 'Authorization has been denied for this request.'

@mytag
	Scenario: Unsuccessful api call without token
	Given a request with url of 'https://api-test.afterpay.dev/'
	When invalid request is posted to api with empty token 'empty' bankAccount 'GB09HAOE91311808002317' and content type of 'application/json' and url of 'api/v3/validate/bank-account'
	Then Api returns error 'Authorization has been denied for this request.'

@mytag
	Scenario: Unsuccessful api call with invalid bankAccount
	Given a request with url of 'https://api-test.afterpay.dev/'
	When invalid request is posted to api with token 'Q7DaxRnFls6IpwSW1SQ2FaTFOf7UdReAFNoKY68L' and invalid bankAccount <bankAccount> and content type of 'application/json' and url of 'api/v3/validate/bank-account'
	Then Api returns bank account validation errors withe corresponding bankAccount <bankAccount> causes error message <errorMessage>
	Examples:
		| bankAccount                                                                      | errorMessage                                      |
		| 123456                                                                           | A string value with minimum length 7 is required. |
		| 123456Rr123456Rr123456Rr123456Rr123456Rr123456Rr123456Rr123456Rr123456Rr123456Rr | A string value exceeds maximum length of 34.      |
		| 12345678                                                                         | false                                             |
		| 12345678Rr                                                                       | false                                             |

@mytag
	Scenario: Unsuccessful api call without bankAccount
	Given a request with url of 'https://api-test.afterpay.dev/'
	When invalid request is posted to api with token 'Q7DaxRnFls6IpwSW1SQ2FaTFOf7UdReAFNoKY68L' and empty bankAccount 'empty' content type of 'application/json' and url of 'api/v3/validate/bank-account'
	Then Api returns bank account validation errors withe corresponding bankAccount <bankAccount> causes error message <errorMessage>
	Examples:
		| bankAccount | errorMessage       |
		| empty       | Value is required. |                                                                   

@mytag
	Scenario: Unsuccessful api with invalid url
	Given a request with an invalid url of 'https://api-test.afterpay.dev/abc'
	When invalid request with invalid url is posted to api with token 'Q7DaxRnFls6IpwSW1SQ2FaTFOf7UdReAFNoKY68L' and 'GB09HAOE91311808002317' and content type of 'application/json' and url of 'api/v3/validate/bank-account'
	Then Api returns error '404 - Not Found'