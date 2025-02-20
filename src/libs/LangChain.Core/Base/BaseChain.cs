using LangChain.Abstractions.Chains.Base;
using LangChain.Abstractions.Schema;
using LangChain.Chains;
using LangChain.Schema;

namespace LangChain.Base;

using System.Collections.Generic;
using LoadValues = Dictionary<string, object>;

/// <inheritdoc />
public abstract class BaseChain : IChain
{
    const string RunKey = "__run";

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public abstract string ChainType();

    /// <summary>
    /// 
    /// </summary>
    public abstract string[] InputKeys { get; }

    /// <summary>
    /// 
    /// </summary>
    public abstract string[] OutputKeys { get; }

    /// <summary>
    /// Run the chain using a simple input/output.
    /// </summary>
    /// <param name="input">The string input to use to execute the chain.</param>
    /// <returns>A text value containing the result of the chain.</returns>
    /// <exception cref="ArgumentException">If the type of chain used expects multiple inputs, this method will throw an ArgumentException.</exception>
    public async Task<string?> Run(string input)
    {
        var isKeylessInput = InputKeys.Length <= 1;

        if (!isKeylessInput)
        {
            throw new ArgumentException($"Chain {ChainType()} expects multiple inputs, cannot use 'run'");
        }

        var values = InputKeys.Length > 0 ? new ChainValues(InputKeys[0], input) : new ChainValues();
        var returnValues = await CallAsync(values);
        var keys = returnValues.Value.Keys;

        if (keys.Count(p => p != RunKey) == 1)
        {
            var returnValue = returnValues.Value.FirstOrDefault(p => p.Key != RunKey).Value;
            return returnValue == null ? null : returnValue.ToString();
        }

        throw new Exception("Return values have multiple keys, 'run' only supported when one key currently");
    }

    /// <summary>
    /// Execute the chain, using the values provided.
    /// </summary>
    /// <param name="values">The <see cref="ChainValues"/> to use.</param>
    /// <returns></returns>
    public abstract Task<IChainValues> CallAsync(IChainValues values);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="inputs"></param>
    /// <returns></returns>
    public async Task<ChainValues> Apply(List<ChainValues> inputs)
    {
        var tasks = inputs.Select(async (input, idx) => await CallAsync(input));
        var results = await Task.WhenAll(tasks);

        return results.Aggregate(new ChainValues(), (acc, result) =>
        {
            foreach (var pair in result.Value)
            {
                acc.Value[pair.Key] = pair.Value;
            }

            return acc;
        });
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="data"></param>
    /// <param name="values"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public static async Task<BaseChain> Deserialize(SerializedBaseChain data, LoadValues? values = null)
    {
        switch (data.Type)
        {
            case "llm_chain":
                {
                    var llmChainType = Type.GetType("Namespace.LLMChain"); // Replace with the actual namespace and class name
                    var deserializeMethod = llmChainType?.GetMethod("Deserialize");

                    return await (Task<BaseChain>)deserializeMethod.Invoke(null, new object[] { data });
                }
            case "sequential_chain":
                {
                    var sequentialChainType = Type.GetType("Namespace.SequentialChain"); // Replace with the actual namespace and class name
                    var deserializeMethod = sequentialChainType.GetMethod("Deserialize");

                    return await (Task<BaseChain>)deserializeMethod.Invoke(null, new object[] { data });
                }
            case "simple_sequential_chain":
                {
                    var simpleSequentialChainType = Type.GetType("Namespace.SimpleSequentialChain"); // Replace with the actual namespace and class name
                    var deserializeMethod = simpleSequentialChainType.GetMethod("Deserialize");

                    return await (Task<BaseChain>)deserializeMethod.Invoke(null, new object[] { data });
                }
            default:
                throw new Exception($"Invalid prompt type in config: {data.Type}");
        }
    }
}
