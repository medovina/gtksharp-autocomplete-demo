using Gdk;
using GLib;
using Gtk;
using GtkSource;
using Application = Gtk.Application;

// A simple autocompletion provider that uses a small fixed set of words.

class Provider : GLib.Object, GtkSource.ICompletionProviderImplementor {
    string[] dictionary = { "apple", "asteroid", "dog", "donut", "pear", "pizza" };

    public string Name => "completions";
    public Pixbuf Icon => null!;
    public string IconName => null!;
    public IIcon Gicon => null!;
    public int InteractiveDelay => -1;
    public int Priority => 0;

    // Display completions interactively as the user types.
    public CompletionActivation Activation => CompletionActivation.Interactive;

    public bool Match(CompletionContext context) => true;
    public Widget GetInfoWidget(ICompletionProposal proposal) => null!;
    public void UpdateInfo(ICompletionProposal proposal, CompletionInfo info) { }
    public bool GetStartIter(CompletionContext context, ICompletionProposal proposal, TextIter iter)
        => false;
    public bool ActivateProposal(ICompletionProposal proposal, TextIter iter) => false;

    public void Populate(CompletionContext context) {
        // Find the text that needs to be autocompleted.
        TextIter end = context.Iter;
        TextIter start = end;
        start.BackwardVisibleWordStart();
        string word_to_complete = start.Buffer.GetText(start, end, false);

        GLib.List list = new GLib.List(typeof(CompletionItem));

        if (word_to_complete != "")
            foreach (string w in dictionary)
                if (w.StartsWith(word_to_complete)) {
                    CompletionItem item = new CompletionItem();
                    item.Label = item.Text = w;
                    list.Append(item.Handle);
                }

        context.AddProposals(new CompletionProviderAdapter(this), list, true);
    }
}

class MyWindow : Gtk.Window {
    public MyWindow() : base("editor") {
        SourceView view = new SourceView();
        Add(view);
        view.Completion.AddProvider(new CompletionProviderAdapter(new Provider()));
        view.Completion.ShowHeaders = false;
    }

    protected override bool OnDeleteEvent(Event e) {
        Application.Quit();
        return true;
    }
}

class Hello {
    static void Main() {
        Application.Init();
        MyWindow w = new MyWindow();
        w.ShowAll();
        Application.Run();
    }
}
