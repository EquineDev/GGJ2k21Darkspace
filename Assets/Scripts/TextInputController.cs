using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
[RequireComponent(typeof(PublishInput))]
public class TextInputController : MonoBehaviour
{
    bool cursor_state_on = false;
    public float cursor_blink_time;
    public Color text_color = Color.green;
    TextMeshProUGUI text_ui;
    Regex cursor_expresson = new Regex(@"\|*$");
    Regex backspace_expression = new Regex(@"(^[\b])|([^\b][\b])");
    Regex commit_expression = new Regex(@"^.*\n");
    string internal_text = "";
    PublishInput emitter;

    // Start is called before the first frame update
    void Start()
    {
        text_ui = GetComponent<TextMeshProUGUI>();
        emitter = GetComponent<PublishInput>();
        if (cursor_blink_time>0)
        {
            InvokeRepeating("BlinkCursor", 0.1f, cursor_blink_time);
        }
    }

    string _cursorStr {
        get {
            return cursor_state_on ? "|" : "";
        }
    }

    // Update is called once per frame
    void Update()
    {
        string newInput = Input.inputString;
        
        internal_text = backspace_expression.Replace(internal_text + newInput, "");

        var command_array = new Stack<string> (
            internal_text
            .Split('\n'));
        internal_text = command_array.Pop();

        string command_text = command_array.SingleOrDefault();
        if (command_text != null)
        {
            // execute command
            emitter.EmitString(command_text);
        }

        text_ui.text = internal_text + _cursorStr;
    }

    void BlinkCursor()
    {
        cursor_state_on = !cursor_state_on;
    }
}
