/*
	adapted from https://github.com/SunnyValleyStudio/procedural_town_unity
*/

using System;
using System.Text;
using UnityEngine;


public enum EncodingLetters
{
	unknown = '1',
	save = '[',
	load = ']',
	draw = 'F',
	turnLeft = '+',
	turnRight = '-'
}


public class LSystemGenerator
{
	// the set of rules defined in the Rule(s) assets
	public LSRule[] rules;
	public int iterationLimit = 1;
	public bool randomIgnoreRuleModifier = true;
	public float chanceToIgnoreRule = 0.3f;

	public LSystemGenerator(LSRule[] rules, int iterationLimit, bool randomIgnoreRuleModifier, float chanceToIgnoreRule)
    {
		this.rules = rules;
		this.iterationLimit = iterationLimit;
		this.randomIgnoreRuleModifier = randomIgnoreRuleModifier;
		this.chanceToIgnoreRule = chanceToIgnoreRule;
    }

	// in the beginning: word --> axiom
	// when GenerateSentence is called recusively from ProcessRules: word --> the rewriting rule from Rule asset 
	public string GenerateSentence(string word, int iterationIndex = 0)
	{

		// when we have done the last iteration, it returns the generated sentence
		// (this is the return to the main application)
		if (iterationIndex >= iterationLimit)
		{
			return word;
		}

		// we use a StringBuilder (we are dynamically modifying a string)
		StringBuilder newWord = new StringBuilder();

		// we append each char in the word in the StringBuilder instance,
		// and we process them
		foreach (var c in word)
		{
			newWord.Append(c);
			ProcessRules(newWord, c, iterationIndex);
		}
		// we return the StringBuilder instance to ProcessRules
		return newWord.ToString();
	}

	private void ProcessRules(StringBuilder newWord, char c, int iterationIndex)
	{
		foreach (var rule in rules)
		{
			// for each rule defined in the Rule asset, we check if the character is the letter (in our example, 'F')
            // we need to rewrite
			if (rule.letter == c.ToString())
			{
				// if we opt for a stochastich L-system, there is a probability the rule is not applied 
				if (randomIgnoreRuleModifier && iterationIndex > 1)
				{
					if(UnityEngine.Random.value < chanceToIgnoreRule)
					{
						return;
					}
				}
				// if we find 'F', we get from Rule asset the string to write instead of 'F',
				// we call recursively GenerateSentence, we increment the iteration counter,
				// and we append the result to the StringBuilder instance
				newWord.Append(GenerateSentence(rule.GetResult(), iterationIndex + 1));
			}
				
		}
	}
}

