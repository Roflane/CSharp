using System.Diagnostics;
using System.Collections.ObjectModel;

namespace TaskManager.Core;

public class ProcessInfo {
    public string ProcessName { get; set; }
    public int ProcessId { get; set; } 
    public string PID => ProcessId.ToString(); 
}

public class TaskManagerCore {
    private static TaskManagerCore _instance = new();
    public static TaskManagerCore Instance => _instance;
    
    public ObservableCollection<ProcessInfo> Processes { get; } = new();
    
    public void GetProcesses() {
        Processes.Clear();
        List<Process> procs = Process.GetProcesses().OrderBy(p => p.Id).ToList();
        foreach (var proc in procs) {
            if (proc.ProcessName == "System" && proc.Id == 4) continue;
            Processes.Add(new ProcessInfo {
                ProcessName = proc.ProcessName + ".exe",
                ProcessId = proc.Id
            });
        }
        
    }

    public void KillProcess(int pid) {
        Process.GetProcessById(pid).Kill();
    }

    public void ChangePriority(int pid, ProcessPriorityClass priority) {
        Process.GetProcessById(pid).PriorityClass = priority;  
    }

    public void SetAffinityOne(int pid) {
        Process.GetProcessById(pid).ProcessorAffinity = new IntPtr(0b1);
    }
    
    public void SetAffinityFull(int pid) {
        Process.GetProcessById(pid).ProcessorAffinity = new IntPtr(1L << Environment.ProcessorCount) - 1;
    }
}