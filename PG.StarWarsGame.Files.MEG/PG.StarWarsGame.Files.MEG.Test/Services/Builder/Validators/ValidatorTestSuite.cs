using System;
using System.Collections.Generic;
using FluentValidation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PG.StarWarsGame.Files.MEG.Test.Services.Builder;

public abstract class ValidatorTestSuite<T>
{
    private IValidator<T>_validator;

    [TestInitialize]
    public void TestSetup()
    {
        _validator = CreateValidator();
    }

    protected abstract IValidator<T> CreateValidator();

    protected abstract IEnumerable<ValidatorTestData<T>> GetValidCases();
    protected abstract IEnumerable<ValidatorTestData<T>> GetInvalidCases();

    [TestMethod]
    public void TestValidCases()
    {
        foreach (var validCase in GetValidCases())
        {
            try
            {
                Assert.IsTrue(_validator.Validate(validCase.TestObject).IsValid, $"Failed Test Case: {validCase.TestName}");
            }
            catch (AssertFailedException)
            {
                throw;
            }
            catch (Exception)
            {
                Console.WriteLine($"Failed Test Case: {validCase.TestName}");
                throw;
            }
        }
    }

    [TestMethod]
    public void TestInvalidCases()
    {
        foreach (var validCase in GetInvalidCases())
        {
            try
            {
                Assert.IsFalse(_validator.Validate(validCase.TestObject).IsValid, $"Failed Test Case: {validCase.TestName}");
            }
            catch (AssertFailedException)
            {
                throw;
            }
            catch (Exception)
            {
                Console.WriteLine($"Failed Test Case: {validCase.TestName}");
                throw;
            }
        }
    }

    public class ValidatorTestData<TK>(TK testObject, string testName)
    {
        public TK TestObject { get; } = testObject;

        public string TestName { get; } = testName;
    }
}