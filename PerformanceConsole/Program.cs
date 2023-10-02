// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.Logging;
using System.Diagnostics;

string[] layersToKeepSeparate = new string[] { "test", "test2" };

CheckLayerSeparationOuter(layersToKeepSeparate, "test3", "test4");

void CheckLayerSeparationOuter(string[] layersToKeepSeparate, params object[] actualParams)
{
    CheckLayerSeparation(layersToKeepSeparate, actualParams);
}

void CheckLayerSeparation(string[] layersToKeepSeparate, object[] actualParams)
{
    using var factory = new LoggerFactory();
    ILogger logger = factory.CreateLogger("SeparationVerifier");

    var stack = new StackTrace();
    var frameToView = 1;
    do
    {
        // Sometimes there will be some interleaved methods between calls, so this helps us pull them out.
        var methodToCheck = stack.GetFrame(frameToView).GetMethod();
        if ((methodToCheck.Module.Name.IndexOf("PerformanceConsole") >= 0))
        {
            break;
        }
    }
    while (++frameToView < stack.FrameCount);

    var callingFrame = stack.GetFrame(frameToView);
    var callingMethod = callingFrame.GetMethod();

    var callingMethodName = callingMethod.Name;
    var callingType = "Method";
    if (callingMethod.IsConstructor)
    {
        callingMethodName = callingMethod.DeclaringType.Name;
        callingType = "Constructor";
    }

    var callerAssemblyName = callingMethod.DeclaringType.Assembly.FullName.Split(",")[0];
    logger.LogDebug("{0} name: {1}, assembly: {2}", callingType, callingMethodName, callerAssemblyName);

    var layerOfCaller = layersToKeepSeparate.ToList().IndexOf(callerAssemblyName);
    if (layerOfCaller < 0)
    {
        // If the constructor is not part of the concerning list, it's safe.
        return;
    }

    var callingParams = callingMethod.GetParameters();
    if (callingParams.Length != actualParams.Length)
    {
        throw new InvalidOperationException($"Calling error for {callingMethodName}: Please pass all constructor parameters to this function. You appear to be missing some.");
    }

    for (var i = 0; i < callingParams.Length; i++)
    {
        var parameterAssemblyName = callingParams[i].ParameterType.Assembly.FullName.Split(",")[0];
        logger.LogDebug("Interface type: {0}, assembly: {1}", callingParams[i].ParameterType.Name, parameterAssemblyName);
        if (layersToKeepSeparate.Contains(parameterAssemblyName) &&
            parameterAssemblyName != callerAssemblyName)
        {
            throw new InvalidOperationException($"Layer break detected: {callingMethodName} constructor requires a {callingParams[i].ParameterType.Name}, which is declared in a separate protected layer.");
        }

        // if (callingParams[i].ParameterType.IsInterface && !callingParams[i].ParameterType.IsAssignableFrom(actualParams[i].GetType()))
        // {
        //    throw new ArgumentException("Calling error: Please pass all constructor parameters to this function. You have incorrect parameters (did you mis-order them?).");
        // }
        // Above code commented out pending investigation into bug: https://microsoft.visualstudio.com/Edge/_workitems/edit/31355486 which cause injection of generic ILogger to fail.
        var argumentType = actualParams[i]?.GetType();
        if (argumentType != null)
        {
            var parameterObjAssemblyName = argumentType.Assembly.FullName.Split(",")[0];
            logger.LogDebug("Parameter type: {0}, assembly: {1}", argumentType.Name, parameterObjAssemblyName);
            var paramClassLayer = layersToKeepSeparate.ToList().IndexOf(parameterObjAssemblyName);
            if (paramClassLayer >= 0)
            {
                if (paramClassLayer < layerOfCaller)
                {
                    throw new InvalidOperationException($"Reverse dependency detected: {callingMethodName} constructor acquired a {argumentType.Name}, which is defined in an higher protected layer.");
                }
                else if (paramClassLayer > layerOfCaller + 1)
                {
                    throw new InvalidOperationException($"Skipped layer dependency detected: {callingMethodName} constructor acquired a {argumentType.Name}, which is defined in an too low of a protected layer.");
                }
            }
        }
    }
}

