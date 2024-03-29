using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using PG.Commons.Attributes;

namespace PG.Commons.Extensibility;

/// <summary>
///     <see cref="IServiceContribution" /> Extensions
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    ///     Convenience method to collect and contribute all <see cref="IServiceContribution" />s.
    /// </summary>
    /// <param name="serviceCollection"></param>
    public static void CollectPgServiceContributions(this IServiceCollection serviceCollection)
    {
        var contributions = AppDomain.CurrentDomain.GetAssemblies()
            .Where(IsValidAssembly)
            .SelectMany(assemblyTypes => assemblyTypes.GetTypes())
            .Where(assemblyType => typeof(IServiceContribution).IsAssignableFrom(assemblyType) &&
                                   assemblyType is { IsClass: true, IsAbstract: false })
            .ToList();
        contributions.Sort((current, other) =>
        {
            var currentOrderAttribute = (OrderAttribute)current.GetCustomAttribute(typeof(OrderAttribute));
            var currentOrder = currentOrderAttribute?.Order ?? OrderAttribute.DefaultOrder;
            var otherOrderAttribute = (OrderAttribute)other.GetCustomAttribute(typeof(OrderAttribute));
            var otherOrder = otherOrderAttribute?.Order ?? OrderAttribute.DefaultOrder;
            return currentOrder.CompareTo(otherOrder);
        });
        contributions.ForEach(contribution =>
        {
            try
            {
                var contributionInstance = (IServiceContribution)Activator.CreateInstance(contribution);
                contributionInstance.ContributeServices(serviceCollection);
            }
            catch (Exception)
            {
                // ignored
            }
        });
    }

    // Apparently there is some race condition when loading types from assemblies which are used for unit testing. 
    // Making CollectPgServiceContributions() synchronized does not solve the problem.
    // With this filter, we just allow assemblies which are from this project.
    // (this is not a security guard as impersonating is still possible)
    private static bool IsValidAssembly(Assembly assembly)
    {
        if (assembly.FullName == typeof(PGServiceContribution).Assembly.FullName)
            return true;
        if (assembly.FullName.StartsWith("PG.StarWarsGame."))
            return true;
        return false;
    }
}