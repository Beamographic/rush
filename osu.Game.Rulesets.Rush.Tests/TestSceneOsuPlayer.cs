// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using osu.Game.Tests.Visual;

namespace osu.Game.Rulesets.Rush.Tests
{
    [TestFixture]
    public class TestSceneOsuPlayer : PlayerTestScene
    {
        public override IReadOnlyList<Type> RequiredTypes => typeof(RushRuleset).Assembly.DefinedTypes.Where(t =>
        {
            if (t.Namespace == null)
                return false;

            return t.Namespace.Contains("UI") || t.Namespace.Contains("Objects");
        }).ToList();

        public TestSceneOsuPlayer()
            : base(new RushRuleset())
        {
        }
    }
}