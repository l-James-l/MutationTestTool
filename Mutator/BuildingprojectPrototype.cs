using Microsoft.Build.Construction;
using Microsoft.Build.Evaluation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutator;

public class BuildingprojectPrototype
{
    public void BuildTestProject()
    {
        string solutionPath = "yousolutionPath.sln";
        SolutionFile solutionFile = SolutionFile.Parse(solutionPath);
        foreach (ProjectInSolution item in solutionFile.ProjectsInOrder)
        {
            Project project = ProjectCollection.GlobalProjectCollection.LoadProject(item.AbsolutePath);
            
            project.SetGlobalProperty("Configuration", "Debug");
            if (project.GetPropertyValue("RootNamespace") == "CppApp5")
            {
                project.Build("Build");
            }
        }
    }
}

