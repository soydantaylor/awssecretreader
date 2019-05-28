#AwsSecretReader

[![CircleCI](https://circleci.com/gh/soydantaylor/awssecretreader.svg?style=svg)](https://circleci.com/gh/soydantaylor/awssecretreader)

This is designed to pull all the AWS Parameter store variables from a single path in your AWS account.

It requires 3 environment variables to be set, or only 1 if you're running it on an AWS resource that has an IAM role.

 - `AWS_ACCESS_KEY_ID`
 - `AWS_SECRET_ACCESS_KEY` and
 - `SSM_PARAMETER_PATH`


If you had some SSM parameters set up like this
 - path `/some-project/some-app/testing/settings/api-key` value `some-guid`
 - path `/some-project/some-app/testing/connection-strings/postgresdb` value `some-connection-string`

and you wanted to get both of these items, then you would set `SSM_PARAMETER_PATH` to
`/some-project/some-app/testing/`

AwsSecretReader will recursively pull all the variables at that path, so you only have to pull them by their name.

Then it's as easy as 

```
var secretReader = SecretReader.Instance;
var fetchedValue = secretReader.GetParameter("api-key");
```
or
```
var secretReader = SecretReader.Instance;
var fetchedValue = secretReader.GetParameter("postgresdb");
```

The values are cached in an in-memory dictionary, so they are only fetched each time the object is instantiated.


